// Testing suite:

RequireScript("imagetest.js");
RequireScript("imagestest.js");
RequireScript("fonttest.js");
RequireScript("windowtest.js");
RequireScript("mousetest.js");
RequireScript("soundtest.js");
RequireScript("surfacetest.js");
RequireScript("spritesettest.js");
RequireScript("savetest.js");

if (!this.JSON) {
	RequireScript("json2.js");
}

const sys_window = GetSystemWindowStyle();
var sys_arrow = GetSystemArrow();
var sys_font = GetSystemFont();

function game()
{
    if (this.SetScaled) SetScaled(true);
    SetFrameRate(0);

	var done = false;
	var menu = new Menu();
		
	menu.addOption("Images", TestImages);
	menu.addOption("Many Images", TestManyImages);
	menu.addOption("Many Primitives", TestManyPrims);
	menu.addOption("Fonts", TestFonts);
	menu.addOption("Windows", TestWindows);
	menu.addOption("Big Window", TestBigWindow);
	menu.addOption("Spritesets", TestSpritesets);
	menu.addOption("Mouse/Input", TestMouse);
	menu.addOption("Surfaces", TestSurfaces);
	menu.addOption("Surface Fill Rate", TestSurfaceFillRate);
	menu.addOption("Color Creation", TestColorCreationSpeed);
	menu.addOption("Music/Sounds", TestMusic);
	menu.addOption("Savefiles", TestSaving);
	menu.addOption("Test Map Anims", function() {
		SetUpdateScript("Update2();");
		if (DoesPersonExist("player")) DestroyPerson("player");
		SetRenderScript("Render2();");
		MapEngine("smalltest.rmp", 60);
	});
	menu.addOption("Test Village map", function() {
	    SetDelayScript(60, "Alert('Hi! From delay script.');");
		CreatePerson("player", "test.rss", false);
		AttachInput("player");
		SetPersonFrameRevert("player", 8);
		SetTalkDistance(16);
		AttachCamera("player");
		SetUpdateScript("Update();");
		SetRenderScript("Render();");
		MapEngine("village.rmp", 60);
	});
	menu.addOption("Test JSON", TestJSON);
	menu.addOption("Test Speed", TestRawSpeed);
	menu.addOption("Exit", Exit);
	
	while (!done) {
		menu.draw(16, 16, 128, 192);
		
		FlipScreen();
		while (AreKeysLeft()) {
			var key = GetKey();
			menu.update(key);
			if (key == KEY_ESCAPE) done = true;
		}
	}
}

function Alert(text) {
    Rectangle(0, 0, GetScreenWidth(), GetScreenHeight(), CreateColor(0, 0, 0, 0));
    sys_font.drawText(0, 0, text);
    FlipScreen();
    GetKey();
}

function TestRawSpeed() {
    var t = GetTime();
    for (var i = 0; i < 10000000; ++i) { }
    t = GetTime() - t;
    Alert(t);
}

function TestJSON()
{
	var s = '{"ships":[{"vx":0,"vy":0,"xx":464.0000000000016,"yy":335.99999999999824,"maxvx":3.83022221559489,"maxvy":3.2139380484326963,"name":"","shipname":"Shuttle","target":null,"target_a":{"rad":0.3430239404207034,"deg":19.65382405805331},"a":{"rad":0.6981317007977318,"deg":40},"lastspeed":5,"maxspeed":5,"speed":0.5284430838679363,"acc":0.5,"last_a_msecs":150,"a_msecs":150,"r_msecs":20,"hp":{"value":25,"max":25},"escort":{"value":0,"max":1},"cap":{"value":15,"max":30},"shields":{"value":0,"max":0},"jumps":{"value":2,"max":2},"shielded":false,"shieldCol":"CreateColor(255,255,255)","shieldAlpha":{"max":0,"value":0,"duration":500},"isEnemy":false,"thrust":false,"equipment":[{"item":0,"amount":0,"time":418824932}],"types":[true],"secondary":-1,"sina":0.6427876096865393,"cosa":0.766044443118978,"image":"shuttle","bitmap":{},"w":48,"h":48,"id":-1,"canTurret":false,"canBeam":false,"jump":0,"jumpSys":{"x":2,"y":20},"targetSys":{"x":30,"y":30,"name":"Hades","neighbors":["Archon","Mede"]},"jumpTime":266307406,"rating":2,"last_a":{"rad":3.839724354387525,"deg":220},"last_set":true,"upkey":true,"hull":null,"thusters":null,"computer":null,"joy":true,"deathTimer":0,"nameType":0,"equips":{"left":{"item":0,"amount":0},"right":{"item":-2,"amount":0},"tech":{"item":-2,"amount":0},"secondary":{"item":-2,"amount":0},"shield":{"item":-2,"amount":0}},"wh_x":-4672.443966187526,"wh_y":-1314.331416492098}],"ship":{"vx":0,"vy":0,"xx":464.0000000000016,"yy":335.99999999999824,"maxvx":3.83022221559489,"maxvy":3.2139380484326963,"name":"","shipname":"Shuttle","target":null,"target_a":{"rad":0.3430239404207034,"deg":19.65382405805331},"a":{"rad":0.6981317007977318,"deg":40},"lastspeed":5,"maxspeed":5,"speed":0.5284430838679363,"acc":0.5,"last_a_msecs":150,"a_msecs":150,"r_msecs":20,"hp":{"value":25,"max":25},"escort":{"value":0,"max":1},"cap":{"value":15,"max":30},"shields":{"value":0,"max":0},"jumps":{"value":2,"max":2},"shielded":false,"shieldCol":"CreateColor(255,255,255)","shieldAlpha":{"max":0,"value":0,"duration":500},"isEnemy":false,"thrust":false,"equipment":[{"item":0,"amount":0,"time":418824932}],"types":[true],"secondary":-1,"sina":0.6427876096865393,"cosa":0.766044443118978,"image":"shuttle","bitmap":{},"w":48,"h":48,"id":-1,"canTurret":false,"canBeam":false,"jump":0,"jumpSys":{"x":2,"y":20},"targetSys":{"x":30,"y":30,"name":"Hades","neighbors":["Archon","Mede"]},"jumpTime":266307406,"rating":2,"last_a":{"rad":3.839724354387525,"deg":220},"last_set":true,"upkey":true,"hull":null,"thusters":null,"computer":null,"joy":true,"deathTimer":0,"nameType":0,"equips":{"left":{"item":0,"amount":0},"right":{"item":-2,"amount":0},"tech":{"item":-2,"amount":0},"secondary":{"item":-2,"amount":0},"shield":{"item":-2,"amount":0}},"wh_x":-4672.443966187526,"wh_y":-1314.331416492098},"curship":0,"days":0,"credits":631,"name":"Jenna Hayes","sex":true,"pic":"Ashley","inventory":[],"quests":[],"travelTo":["Hades"]}'
	var t = GetTime();
	for (var i = 0; i < 10; ++i) {
		JSON.parse(s);
	}
	t = GetTime() - t;
	Alert(t);
}

function Render()
{
	sys_font.drawText(0, 0, GetPersonX("player") + "," + GetPersonY("player") + "  " + GetTalkDistance());
}

function Render2()
{
	sys_font.drawText(0, 0, GetCameraX() + ", " + GetCameraY());
}

var testperson = new MapEntity("barnum");

function Update()
{
	testperson.randomMove();
	while (AreKeysLeft()) {
		switch (GetKey()) {
			case KEY_ENTER: ExitMapEngine(); break;
			case KEY_SPACE: testperson.start = true; break;
		}
	}
}

function Update2()
{
	Update();
	
	if (IsKeyPressed(KEY_RIGHT))
		SetCameraX(GetCameraX() + 2);
	if (IsKeyPressed(KEY_LEFT))
		SetCameraX(GetCameraX() - 2);
	if (IsKeyPressed(KEY_UP))
		SetCameraY(GetCameraY() - 2);
	if (IsKeyPressed(KEY_DOWN))
		SetCameraY(GetCameraY() + 2);
}

function Menu()
{
	this.items   = [];
	this.index   = 0;
	this.top     = 0;
	this.visible = 0;
}

Menu.prototype.addOption = function(name, callback) {
	this.items.push({name: name, callback: callback});
}

Menu.prototype.draw = function(x, y, w, h) {
	sys_window.drawWindow(x, y, w, h);
	
	this.visible = h / 16;
	for (var i = 0; i < this.visible; ++i) {
	    if (i + this.top >= this.items.length) break;
		sys_font.drawText(x + 16, y + i*16, this.items[i + this.top].name);
	}
	
	sys_arrow.blit(x, y + (this.index - this.top) * 16);
}

Menu.prototype.update = function(key) {
	switch(key) {
	    case KEY_UP:
	        if (this.index > 0) this.index--;
	        if (this.index < this.top) this.top--;
		break;
	    case KEY_DOWN:
	        if (this.index < this.items.length - 1) this.index++;
	        if (this.index >= this.top + this.visible) this.top++;
		break;
		case KEY_ENTER:
			this.items[this.index].callback();
		break;
	}
}

function ShowText(text)
{
	var done = false;
    while (!done) {
		RenderMap();
		sys_window.drawWindow(16, 16, GetScreenWidth()-32, 48);
		sys_font.drawText(16, 16, text);
		
		FlipScreen();
		
		while (AreKeysLeft()) {
		    if (GetKey() == KEY_ENTER) done = true; 
		}
	}
}

function Walk(name, dir, tiles)
{
	var face = null;
	var point = {x: 0, y: 0};
	switch (dir)
	{
	    case COMMAND_MOVE_NORTH: face = COMMAND_FACE_NORTH; point.y = -1; break;
	    case COMMAND_MOVE_SOUTH: face = COMMAND_FACE_SOUTH; point.y =  1; break;
	    case COMMAND_MOVE_EAST: face = COMMAND_FACE_EAST; point.x =  1; break;
	    case COMMAND_MOVE_WEST: face = COMMAND_FACE_WEST; point.x = -1; break;
	}
	
	if (face != null) QueuePersonCommand(name, face, true);
	for (var i = 0, total = tiles * GetTileWidth(); i < total; ++i) {
	    QueuePersonCommand(name, dir, false);
	}
	
	return point;
}

function MapEntity(name)
{
	this.name = name;
	this.distance = 32;
	this.startX = -1;
	this.startY = -1;
	this.start = false;
}

MapEntity.prototype.randomMove = function()
{
	if (!this.start) return;
	if (!IsCommandQueueEmpty(this.name)) return;
	
	var x = GetPersonX(this.name);
	var y = GetPersonY(this.name);
	if (this.startX < 0 || this.startY < 0) {
		this.startX = x;
		this.startY = y;
	}
	
	var arr = [COMMAND_MOVE_NORTH, COMMAND_MOVE_SOUTH, COMMAND_MOVE_EAST, COMMAND_MOVE_WEST, COMMAND_WAIT];
	var i = Math.floor(Math.random() * arr.length);
	var future = Walk(this.name, arr[i], 1 + Math.floor(Math.random()*2));

	var dx = this.startX - (x + future.x * 16);
	var dy = this.startY - (y + future.y * 16);
	if (Math.sqrt(dx * dx + dy*dy) > this.distance) { ClearPersonCommands(this.name); }
}