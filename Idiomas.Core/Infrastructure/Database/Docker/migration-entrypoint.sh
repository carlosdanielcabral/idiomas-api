#!/bin/sh
set -e

COMMAND="${1:-migrate}"

case "$COMMAND" in
  create)
    if [ -z "${2:-}" ]; then
      echo "Usage: $0 create <migration-name>"
      exit 1
    fi

    NAME="$2"
    dotnet ef migrations add "$NAME" --context ApplicationContext --output-dir Infrastructure/Database/Migrations
    ;;
  migrate)
    dotnet ef database update --context ApplicationContext
    ;;
  rollback)
    if [ -z "${2:-}" ]; then
      echo "Usage: $0 rollback <target-migration>"
      exit 1
    fi

    TARGET="$2"
    dotnet ef database update "$TARGET" --context ApplicationContext
    ;;
  *)
    echo "Usage: $0 {create <name>|migrate|rollback <target>}"
    exit 1
    ;;
esac
