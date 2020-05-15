---
layout: default
title: インストール
---

## インストール

インストールにはClickOnceを利用しています．コードサイニング証明書が比較的高価であるためオレオレ証明にしています．そのためインストールの事前準備として証明書のインストールが必要になります．

参考：[https://qiita.com/do-gugan/items/65ab430fa6f47450218d](https://qiita.com/do-gugan/items/65ab430fa6f47450218d)

### 証明書のインストール

利用ユーザ毎に以下の手順で証明書をインストールしてください．

1. srats2017.cerをダウンロード [download](https://gist.github.com/okamumu/eacab78f6099a7df01eedca56712b73d/raw/b07852c8625283a51d4e9b91ad73fe9db4c4b5f1/srats2017.cer)
1. ダウンロードしたsrats2017.cerをダブルクリックしてウィザードを起動
1. 「証明書をすべて次のストアに配置する」にチェック
1. 証明書ストアの「参照」をクリック
1. 「信頼されたルート証明機関」を選択してインストール

### インストール

下記からsetup.exeを実行またはダウンロード&実行してください．

[https://swreliab.github.io/SRATS2017/installer/setup.exe](https://swreliab.github.io/SRATS2017/installer/setup.exe)

証明書をインストールした場合でも完全に警告がなくなることはないので下記の対応をお願いします．

- 「WindowsによってPCが保護されました」のメッセージが出た場合には「詳細情報」をクリックして「実行」をしてください．
- インストーラーから「発行元を確認できません」というメッセージが出た場合には「インストール」を選択してください．

Excelを起動し「アドイン」のパネルに SRATS があればインストール完了です．

### 動作環境

確認している動作環境は以下の通りです．

- OS
  - Windows 10 64bit Education
- Excel
  - Microsoft Excel for Office 365 MSO (16.0.11929.20708) 64bit

### アンインストール

「設定」->「アプリと機能」から SRATSAddIn を選んで「アンインストール」を押してください．

