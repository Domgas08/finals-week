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

## Updating after a merge to main

```
cd finals-week
git pull
docker compose -f deploy/docker-compose.prod.yml up -d --build
```

Only `web` is rebuilt. The database and the login keys are kept, so nobody loses data or gets logged out.

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
