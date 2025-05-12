# 核心架構設計
## 設計目標
- 符合SOLID原則
    - S - 單一職責 (Single Responsibility)
    - O - 開放封閉 (Open-Closed)
    - L - 里氏替換 (Liskov Substitution)
    - I - 介面隔離 (Interface Segregation)
    - D - 依賴反轉 (Dependency Inversion)
- CQRS導入
    - 分離設備指令與數據查詢－能在已有資料回覆的就不往設備上要。
- 事件驅動架構
    - 設備資料通過 `Event Action` 發布，訂閱者自行處理事件。
    - 通常用在 `logger` 與 `異常處理`。
    - 流程的 `Pause` 與 `Emergency Stop` 會使用到，因為涉及 `Graceful stop` 而非僅粗暴斷電。
- 依賴注入(Dependency Injection)
    - 設備樹的部分尤其重要，這會建立依賴關係表。

## 核心
- 核心負責管理設備樹、整體流程控制、通訊與GUI Render。
- 提供設備樹。
- 提供流程管理與封裝為`Task`，主要加速來自採非同步通訊。
- 提供 `logger` 供外部設備註冊。
- 提供系統資訊，如 `tree()` , `errors()` 等介面以通知外部(主動通知或被動通知)。
- 提供 GUI plugin 註冊與調用，其預計與 `BaseNode<T>` 協作。

### 介面
- 事件接口，設備通知時觸發 event action 執行對應事件。

### 流程系統－TaskManager
- 排程－`Queue<List<Task>>`
- TaskExecutor
    - 負責流程的執行與暫停、終止。
    - `CancellationTokenSource` 用於急停 `Task`。

## Plugins
- 設備(Device)
    - 繼承自核心提供的 `Interface Class`。
    - 設備依賴核心中提供的介面，要求核心暴露介面供設備使用，此為設備樹的葉節點。
    - 獨立設計為DLL。
- 流程(Flow)
    - 將 `Flow` 獨立為DLL，與設備一樣。
    - 要求核心暴露設備，再進行流程操作。
    - 通過 `event action` 回傳資訊。
- MES整合
    - 設備需與外部MES通訊，避免緊耦合，採用plugin設計。

## Schedler
- 排程系統，週期性或單次觸發 `Task` 。 (待定)
- 支援註冊與移除。 (待定)

## 設備與設備樹－DeviceTree
- Base Device Class
- 使用DI，註冊至上層節點，最終會形成樹狀結構表。
- SearchDevice，回傳Device或null。
- 設備異常時如果不是Root Node，向上冒泡。

## Basic GUI
- 核心可以在不啟動GUI情況下運作。
- 顯示設備控制測試介面。
- 產出的結果介面。
- 設備紀錄。
- 設備log file。

---
