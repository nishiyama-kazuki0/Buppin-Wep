# JA_ZENNOH_BLAZOR_APP
JA_ZENNOH_BLAZOR_APP

# Introduction 
"JA_ZENNOH_BLAZOR_APP"はＪＡ全農青果センター株式会社殿 大阪センター向け
BlazorAppです。

# Features 
"JA_ZENNOH_BLAZOR_APP"は以下リリース方法で実装します。

* WASM

理由:実行環境は屋外での使用を想定しており、NW接続不安定時にも耐えれるようにしたいため。

# Requirement
"JA_ZENNOH_BLAZOR_APP"の必須ライブラリ

* .NET 7
* Radzen.Blazor

# Note
ブランチについて
リリース用ブランチへのマージはチェリーピックを使用する。
* master : リリース用
* develop : 開発用
* sample : サンプル画面用のブランチ。AzureStaticAppsにgithubActionワークフローで自動ビルドされる

本番リリースでのWASMは圧縮配信を利用する。
ネットワーク環境が不安点な場合、複数機器でWASMダウンロードを行うと、ネットワーク帯域を圧迫してしまうため。
したがって、適用されるファイルは拡張子.gz/.brである点に注意。特にappsetting.json.gz/.br

# Build and Test
TODO: Describe and show how to build your code and run the tests. 

# Contribute


# License
"JA_ZENNOH_BLAZOR_APP" is Confidential.