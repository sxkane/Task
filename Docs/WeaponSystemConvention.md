# Weapon System Convention

## 目标

- 统一武器系统命名风格，降低维护成本。
- 解决武器稀有度缺失（如没有 `Common`）的兼容问题。
- 建立武器标签与套装层级（2~6）基础规则。
- 明确 `WeaponLoadoutEntry` 的拆分路径与拆分时机。

## 命名规范（统一风格）

- `Configure(...)`
  - 注入引用、静态配置，不做运行态逻辑。
- `InitializeRun(...)`
  - 每局初始化，允许接收本局参数（如初始武器列表）。
- `ResetRun()`
  - 清理本局状态，回到可重新开局状态。
- `BeginPhase() / EndPhase()`
  - 阶段开关（战斗开始/暂停/结束）。
- `SetPageVisible(bool)`
  - UI 显示切换。
- `RefreshView() / RebuildSlots()`
  - UI 数据刷新与列表重建。
- `HandleXxx(...)`
  - 事件回调处理函数。

## 稀有度缺失处理规则

- `WeaponData.CreateEntry(rarity)` 不再假设目标稀有度一定存在。
- 当请求稀有度缺失时，按以下顺序回退：
  1. 优先向上找最近可用稀有度；
  2. 若向上没有，再向下找最近可用稀有度。
- `CreateDefaultEntry()` 使用该武器“最低可用稀有度”，不强制 `Common`。
- `CanUpgrade()/CreateUpgradedEntry()` 使用“下一个可用稀有度”，允许跳级（例如 Rare -> Legendary）。

## 武器标签与套装规则

- 每把武器支持多个 `WeaponTag`。
- 套装层级按“同标签武器数量”计算，范围 `0~6`，核心触发区间为 `2~6`。
- 套装加成通过 `WeaponSetBonusData`（ScriptableObject）配置，不在代码里写死数值。
- `WeaponManager` 只负责：统计标签数量、解析激活层级、应用/移除对应 modifier。

## `WeaponLoadoutEntry` 拆分建议

### 现在不急拆的原因

- 当前字段量小（`weaponData + rarity`），逻辑负担可控。
- 仍处于武器系统收敛期，先稳定规则比过早拆分类更重要。

### 什么时候该拆

- 出现以下任一情况时建议拆分：
  - 需要记录“来源/词条/临时附魔/强化历史”等运行态信息；
  - 需要把“商店展示条目”和“战斗实例条目”分离；
  - `WeaponLoadoutEntry` 频繁承担 UI、存档、战斗三类职责。

### 推荐拆分方式

- `WeaponSelectionEntry`：偏 UI/商店，描述“可购买或可选的武器方案”。
- `WeaponRuntimeEntry`：偏战斗运行态，描述“本局实际生效配置”。
- `WeaponLoadoutEntry`：保留为轻量兼容层，逐步迁移调用方。

### 当前迁移状态（已落地）

- 保留 `WeaponLoadoutEntry` 作为主兼容类型（不破坏现有 ScriptableObject/序列化资产）。
- 新增 `WeaponSelectionEntry` 作为新命名入口，后续新代码优先使用该类型。
- 新增 `WeaponRuntimeEntry` 用于运行态语义（战斗中挂载到武器实例的数据）。
- 迁移节奏：先新增类型与工厂方法，待调用侧稳定后再评估是否彻底替换。
- 当前链路已对齐：`CharacterSelect -> GameSession -> PreparingPhase -> WeaponManager.InitializeRun(...)`。

## “缺失稀有度”最终策略

- 任何入口（商店生成、初始武器、运行时升级）都不直接假设目标稀有度存在。
- 统一通过 `WeaponData` 做归一：
  - `GetClosestAvailableRarity(requested)`：缺档时就近回退（先向上再向下）。
  - `TryGetNextAvailableRarity(current, out next)`：升级时找下一个存在的档位。
  - `TryCreateEntry(...)`：安全创建，支持无效配置时返回失败。
- 例子：
  - 武器没有 `Common`，请求 `Common` 会自动落到 `Rare`（若存在）。
  - 武器缺 `Epic`，`Rare` 升级会直接跳到 `Legendary`（若存在）。
