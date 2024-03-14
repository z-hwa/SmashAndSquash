### 2024.2.4
	更換UI設定  
		把UI的Canvas設定為隨螢幕大小自適應  
	背包系統完成  
		可設定出戰單位  
		點擊動畫添加  
	戰鬥時敵我雙方的陣營標示完成  
	簡化關卡設計方面的操作  
		現在只需要設定敵人數量  
		新增敵人物件，放進mapinfo的enemy位置，並添加該敵人的unitBook就完成敵人添加了  
	完成遊戲中的存檔系統  
### 2024.1.31  
	創建開發者頁面  
		並設置戰鬥、背包單位添加、出擊單位添加等測試功能  
	建造背包系統  
	創建單位註冊系統  
		以後新的單位都必須在這裡註冊  
	修改戰鬥系統  
		只有玩家和敵人結束時，會各自檢查單位是否死亡->都會檢查  
		現在的玩家出生點由地圖決定  
	主頁面的導航欄系統完成  
	現在的主頁面以及戰鬥系統已經可以串通了  
### 2024.1.11  
	reset layout of mainpage  
		cutting each component of ui in main scene to proper size  
	add change page fun. to mainscene  
### 2023.10.30  
	finish camera tracing(Unit)  
	link battle system -> camera system and player control system  
	finish unit instantiated in battle system(also init of battle env.)  
### 2023.11.3  
	debug: rebound magnitude fixed to right situation  
	setting mass and drag of rigid2D of unit  
### 2023.11.4   
	deal with unit data class and loading unit data in pre-combat phase   
### 2023.11.5  
	optimize camera tracing  
		Add recorder to tracing target  
		and let camera tracing the recorder  
		Add tracing error in traced logic to smooth tracing effct  
	Impact bounce effect between units are fixed  
		when a unit A smash anotheer unit B, A will be rebounded, B will be hit back  
	Combat round logic completed  
		start->player->enemy->...  
	Enemy default agent is completed  
		which wrote in unit templete A000  
### 2023.11.14  
	Improve picture quality of unit  
		change unit sprite from square to circle  
		using medibang cut picture  
	finish isPlayerWin, isPlayerLose judge  
		and switch unit on turn logic are finished  
		check unit's gameobject is active  
### 2023.11.18  
	main page design 1st edition finish  
### 2023.11.22  
	create chinese font in unity and finish mainPage, package, summon, other buttom  
	set screen size with max portrait size  
		has best resolution  
