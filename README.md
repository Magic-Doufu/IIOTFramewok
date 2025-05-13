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

## 測試

- [X] 設備樹
- [X] 流程系統
- [ ] 設備操作
- [ ] 流程封裝
- [ ] GUI Framework
- [ ] GUI Plugin

## 核心
核心負責管理設備樹、整體流程控制、通訊與GUI Render。

- [X] 設備樹
- [X] 流程系統(異步)
- [ ] 流程封裝器
- [ ] GUI Framework

## 設備樹－BaseNode
- `BaseNode(BaseNode? parent, string? NickName)` 基類，用於提供設備樹。
- `BaseNode.find(string NickName)` 用於通過暱稱尋找設備。
- `Display()` 用於顯示樹狀結構。
- `BaseNode<T>` 類，用於分化出不同周邊設備，亦可用於型別檢查。
- `OnDeviceStatusChanged` Action 提供設備狀態變更時的向上冒泡機制。
- [待定] 提供 GUI plugin 註冊與調用，與 `BaseNode<T>` 協作。

## 流程系統－TaskManager
- 排程使用`ConcurrentQueue<List<Func<CancellationToken, Task>>>`
- 送入`awaitable`的Task，帶有一個`CancellationToken`的參數就可以自動封裝進List。
- 使用`params`方式新增，不用每次都寫`new List ...`。
- TaskExecutor
    - 負責流程的執行與終止。
    - `CancellationTokenSource` 用於急停 `Task`。
    - 急停後的`Graceful`退出。

- 待實作：Graceful退出的註冊點

## Plugins－[規劃中]
- 設備(Device)
    - 繼承自核心提供的 `BaseNode<T>`類。
    - 泛型設備
    - 設備依賴核心中提供的介面，要求核心暴露介面供設備使用，此為設備樹的葉節點。

- 流程(Flow)
    - 將 `Flow` 獨立為DLL，與設備一樣。
    - 要求核心暴露設備，再進行流程操作。
    - 通過 `event action` 回傳資訊。

- MES整合
    - 設備需與外部MES通訊，避免緊耦合，採用plugin設計。

## Schedler－[待定] 
- 排程系統，週期性或單次觸發 `Task` 。 (待定)
- 支援註冊與移除。 (待定)

## Basic GUI－[待定] 
- 核心可以在不啟動GUI情況下運作。
- 顯示設備控制測試介面。
- 產出的結果介面。
- 設備紀錄。
- 設備log file。

---
