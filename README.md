# MaterialHandlingManagementApp
MaterialHandlingManagementApp

# Introduction 
"MaterialHandlingManagementApp"はDBで定義されているパラメータを基にカスタマイズに柔軟性を兼ね備えることを目的とした
BlazorAppです。

CycleProcessAppはPLCとMCProtocol(SLMP)で高速通信を行うことを目的としたWorkerServiceアプリです。

# Features 
"MaterialHandlingManagementApp"BlazorAppは以下リリース方法で実装します。

* WASM

理由:実行環境は屋外での使用を想定しており、NW接続不安定時にも耐えれるようにしたいため。

# Requirement
"MaterialHandlingManagementApp"の必須ライブラリ

* .NET 8
* Radzen.Blazor
* DynamicExpresso

# Note
ブランチについて
リリース用ブランチへのマージはチェリーピックを使用する。
* master : リリース用
* develop : 開発用

本番リリースでのWASMは圧縮配信を利用する。
ネットワーク環境が不安点な場合、複数機器でWASMダウンロードを行うと、ネットワーク帯域を圧迫してしまうため。
したがって、適用されるファイルは拡張子.gz/.brである点に注意。特にappsetting.json.gz/.br

# Build and Test
TODO: Describe and show how to build your code and run the tests. 

# Contribute


# License
"ExpressionDBBlazorWebApp" is Confidential.