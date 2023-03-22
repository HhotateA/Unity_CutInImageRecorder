# CutInImageRecorder

## About
Untiyのカメラ映像をAPNG,GIF,WebPの各型式のアニメーションテクスチャとして保存するアドオンです。
作者が、ココフォリア（https://ccfolia.com/）の演出を作製する為に作製しました。
他動画作成ツールよりUnityの方が使い慣れてるぜ！って方、Unityの資産を活かしたい方向け

## HowToUse
1. Hierarchyを右クリックし、Video>ImageRecorderを選択。
2. Unity上部の「▷」マークをクリックしてSceneをPlay状態にする。
3. ImageRecorderのCutInImageRecorderコンポーネントの「Record」ボタンをおして動画撮影開始。
4. アニメーションテクスチャがAssets下に生成される。

## 機能説明
・Resolution : アニメーションテクスチャの解像度。
・Loop : アニメーションをループさせるか。（チェックを外すと、1回の再生後静止画になります）
・FPS : 1秒間に何フレーム描画するか。
・RecordSecond/Frame : 録画秒数。フレーム数が多くなると録画データ量が肥大化します。
・OutputPath : 出力先フォルダ。
・OutputType : 出力形式。（WebPがおすすめです）
・PlayToRecord : UnityシーンをPlay状態にした時に自動的に録画を開始します。
・Delay : 録画開始ボタンがおされてから実際に録画が開始されるまでの時間。

## LICENSES
### APNG Assembler
virsion:apngasm-2.91-bin-win32
lisence:zlib/libpng License
url:https://sourceforge.net/projects/apngasm/
### APNG Assembler
virsion:apng2gif_gui-1.8-bin-win32
lisence:zlib/libpng License
url:https://sourceforge.net/projects/apng2gif/
### img2webp
virsion:libwebp-1.3.0-windows-x64
lisence:https://www.webmproject.org/license/software/
url:https://developers.google.com/speed/webp/download?hl=ja

## Auther
@HhotateA_xR 20230322