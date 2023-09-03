# cshrap_udp

開発環境
- Docker

## 開発用コンテナの起動
```sh
docker compose --profile develop up
```

## 開発用コンテナの接続
 ```sh
# 起動中の開発用コンテナ名を確認
docker ps

# コンテナ接続 NAMES例：cshrap_udp-udp-develop-1
docker exec -it <NAMES> bash 
 ```

## 　アプリコンテナの起動
```sh
docker compose --profile app up
```