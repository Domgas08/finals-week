#!/usr/bin/env bash
# Deploys new commits from GitHub. Run every minute by finals-week-update.timer.
#
# 1. Ask GitHub for the latest commit on main.
# 2. If the server already runs that commit, stop. Nothing to do.
# 3. Otherwise move the checkout to that commit and rebuild with Docker Compose.
#    If the build fails, the old containers keep running.
#
# Run it by hand to deploy now:  bash deploy/auto-update.sh
# Logs when run by the timer:    journalctl -u finals-week-update -f

set -euo pipefail

BRANCH="${DEPLOY_BRANCH:-main}"
REPO_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
COMPOSE_FILE="$REPO_DIR/deploy/docker-compose.prod.yml"

cd "$REPO_DIR"

# Never run twice at the same time (a build can take longer than a minute).
exec 9>/tmp/finals-week-update.lock
if ! flock -n 9; then
    echo "Another update is still running. Skipping."
    exit 0
fi

git fetch --quiet origin "$BRANCH"

current="$(git rev-parse HEAD)"
latest="$(git rev-parse "origin/$BRANCH")"

if [ "$current" = "$latest" ]; then
    exit 0
fi

echo "New commit on $BRANCH: $(git log -1 --format='%h %s' "$latest")"

# Make the checkout exactly equal to GitHub. deploy/.env is ignored by git,
# so it is not touched.
git checkout --quiet -B "$BRANCH" "origin/$BRANCH"
git reset --quiet --hard "origin/$BRANCH"

if [ ! -f "$COMPOSE_FILE" ]; then
    echo "ERROR: $COMPOSE_FILE does not exist on $BRANCH. Is the hosting PR merged?"
    exit 1
fi

docker compose -f "$COMPOSE_FILE" up -d --build --remove-orphans

# Delete old images left over from previous builds, so the disk does not fill up.
docker image prune -f > /dev/null

echo "Deployed $(git log -1 --format='%h %s')"
