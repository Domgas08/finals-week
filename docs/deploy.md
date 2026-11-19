# Deploying Finals Week to our server

The app runs in three Docker containers on the Ubuntu VM:

| Container | What it does |
|---|---|
| `db` | PostgreSQL 17. Data is kept in the `db-data` volume. |
| `web` | The ASP.NET Core app, built from the `Dockerfile` in the repo root. Applies EF migrations on startup. |
| `tunnel` | `cloudflared`. Connects out to Cloudflare and brings the site's traffic to `web`. |

Browser → Cloudflare (HTTPS) → tunnel → `web` (HTTP, port 8080) → `db`.

No port is opened on the server, so this stack cannot clash with the other services or tunnels on the VM.

## What you need

- SSH access to the VM, with Docker and the Compose plugin (`docker compose version`).
- A domain in Cloudflare. The site gets a subdomain of it, for example `finals.<your domain>`.

## 1. Create the tunnel in Cloudflare (once)

1. Cloudflare dashboard → Zero Trust → Networks → Tunnels → **Create Tunnel**.
2. Type: **Cloudflared**. Name: `finals-week`.
3. When asked for the environment, choose **Docker**. Copy the long token after `--token` in the command it shows.
   Do not run that command. The compose file runs the connector for us.
4. Add a **public hostname** (in the newer dashboard: a published application route):
   - Subdomain: for example `finals`. Domain: yours.
   - Service type: **HTTP**. URL: **`web:8080`**.

   `web` is the name of the app container. The tunnel container runs next to it in the same Docker network, so it finds it by that name. Do not use `localhost` here.

## 2. First deployment on the VM

```
git clone https://github.com/Domgas08/finals-week.git
cd finals-week
cp deploy/.env.example deploy/.env
nano deploy/.env
```

Set `TUNNEL_TOKEN` to the token from step 1, and `POSTGRES_PASSWORD` to the output of `openssl rand -hex 24`.

Then build and start everything:

```
docker compose -f deploy/docker-compose.prod.yml up -d --build
```

The first build takes a few minutes. Check that the three containers run:

```
docker compose -f deploy/docker-compose.prod.yml ps
```

In the Cloudflare dashboard the `finals-week` tunnel should turn **Healthy**. Open `https://finals.<your domain>`, register an account and log in.

The stack is called `finals-week`, so it also shows up in Portainer under that name.

## Updating by hand after a merge to main

With automatic updates (below) you do not need this.


```
cd finals-week
git pull
docker compose -f deploy/docker-compose.prod.yml up -d --build
```

Only `web` is rebuilt. The database and the login keys are kept, so nobody loses data or gets logged out.

## Automatic updates

Both ways below run the same script, `deploy/auto-update.sh`. It fetches `main` from GitHub. If the server is not on the newest commit, it moves to it and runs `up -d --build`. If the build fails, the old version keeps running.

Use **one** of the two ways, not both. The timer deploys every new commit, even one whose tests failed.

Both need the hosting PR merged into `main` first, because the script deploys `main`.
With either one on, do not edit tracked files on the server. Every update resets the checkout to match GitHub. `deploy/.env` is not tracked, so it is safe.

### Way A (recommended): deploy from GitHub through n8n

After a merge to `main`, the CI workflow builds and runs the tests. Only if they pass, its `deploy` job calls an n8n webhook. n8n answers at once and runs the script on the server over SSH.
GitHub reaches n8n through n8n's own Cloudflare Tunnel, so no port is opened. The SSH key n8n uses can only run the deploy script, nothing else.

**1. A deploy-only SSH key (on the server):**

```
ssh-keygen -t ed25519 -N "" -C n8n-deploy -f ~/n8n-deploy-key
echo "command=\"bash /home/ubuntu/projects/finals-week/deploy/auto-update.sh\",no-port-forwarding,no-pty,no-agent-forwarding,no-X11-forwarding $(cat ~/n8n-deploy-key.pub)" >> ~/.ssh/authorized_keys
```

The `command="..."` part means: whoever logs in with this key runs the deploy script and is logged out. They get no shell.

Test it. It should run the script and return without a shell:

```
ssh -i ~/n8n-deploy-key ubuntu@localhost
```

Print the private key, copy it into n8n in the next step, then delete both key files:

```
cat ~/n8n-deploy-key
rm ~/n8n-deploy-key ~/n8n-deploy-key.pub
```

**2. A random token for the webhook:**

```
openssl rand -hex 32
```

**3. The workflow in n8n.** Create a new workflow `finals-week-deploy` with two nodes:

- **Webhook**
  - HTTP Method: `POST`
  - Path: `finals-week-deploy`
  - Authentication: **Header Auth**. New credential: Name `X-Deploy-Token`, Value: the token from step 2.
  - Respond: **Immediately**. The build takes minutes, and Cloudflare cuts requests that wait longer than about 100 seconds.
- **SSH** (connected after the Webhook)
  - Operation: Execute Command. Command: `deploy` (any text works, the key always runs the script).
  - New credential, type **Private Key**: Host: the server's LAN IP (`hostname -I`, first address), Port `22`, Username `ubuntu`, Private Key: the key from step 1.
    If n8n runs in Docker on this same VM, `localhost` will not work, because it means the n8n container.

Activate the workflow. Copy the webhook's **Production URL**, for example `https://<n8n domain>/webhook/finals-week-deploy`.

**4. GitHub secrets.** Repository → Settings → Secrets and variables → Actions → New repository secret:

- `DEPLOY_WEBHOOK_URL`: the Production URL from step 3
- `DEPLOY_WEBHOOK_TOKEN`: the token from step 2

Until `DEPLOY_WEBHOOK_URL` is set, the `deploy` job only prints that nothing was deployed.

**Check it:** after the next merge, the Actions tab shows a `deploy` job after `build-and-test`. n8n → Executions shows the run, with the script's output in the SSH node.

**If the deploy job fails:**
- **403 from Cloudflare:** the n8n domain is protected (Cloudflare Access, or bot protection). Allow the path `/webhook/finals-week-deploy`.
- **403 from n8n:** the token in GitHub and in the n8n credential differ.
- **404:** the workflow is not active, or the URL is the Test URL instead of the Production URL.

### Way B: the server checks GitHub every minute

No n8n and no secrets. The server itself checks `main` every minute. It does not wait for the tests.

```
cd ~/projects/finals-week
sudo cp deploy/finals-week-update.service deploy/finals-week-update.timer /etc/systemd/system/
sudo systemctl daemon-reload
sudo systemctl enable --now finals-week-update.timer
```

The service expects the repo in `/home/ubuntu/projects/finals-week` and runs as `ubuntu`. If that changes, edit `User=` and `ExecStart=` in `/etc/systemd/system/finals-week-update.service`.

Check it with `systemctl list-timers finals-week-update.timer` and `journalctl -u finals-week-update -f`.
Turn it off with `sudo systemctl disable --now finals-week-update.timer`.

### Deploy right now, by hand

```
bash ~/projects/finals-week/deploy/auto-update.sh
```

## Looking at logs

```
docker compose -f deploy/docker-compose.prod.yml logs -f web
docker compose -f deploy/docker-compose.prod.yml logs -f tunnel
```

Press Ctrl+C to stop following. Portainer shows the same logs.

## When something goes wrong

- **The tunnel stays Inactive or Down.** Check the `tunnel` logs. Usually the token in `deploy/.env` is wrong or incomplete.
- **Cloudflare shows "Bad gateway" (502).** The tunnel works, but it cannot reach the app. Check that the hostname's URL is `web:8080` (not `localhost`), and check the `web` logs.
- **`web` keeps restarting.** Check the `web` logs. A wrong `POSTGRES_PASSWORD` shows up as an authentication error.
  If you changed the password after the first start, the old one is still stored in the `db-data` volume.
- **The page loads but freezes after a click.** Blazor needs WebSockets. In Cloudflare: your domain → Network → WebSockets must be on.

## Backing up the database

```
docker compose -f deploy/docker-compose.prod.yml exec db pg_dump -U finalsuser finals-week > backup.sql
```

## Things to know

- `deploy/.env` holds the tunnel token and the database password. It is ignored by git. Never commit it.
- The `docker-compose.yml` in the repo root is only for local development and is not used here.
- `docker compose ... down -v` deletes the volumes, which means all users and decks. Do not run it on the server unless you mean it.
