
var font = LoadFont("test.rfn");
var transparent = CreateColor(0,0,0,0);
var white = CreateColor(0,255,0);
var blue = CreateColor(150,150,255);
var ScreenWidth = GetScreenWidth();
var ScreenHeight = GetScreenHeight();
var FontHeight = font.getHeight();

function game2(){
DrawRG(ScreenWidth/2,ScreenHeight/2,2,5,5);
DrawRG(ScreenWidth/2,ScreenHeight/2,2,5,5);
}

function DrawRG(x,y,spd,size,frnd){
var dx = new Array()
var dy = new Array()
var step = 0
var rnd = 0
var dir = Math.round(Math.random() * 4)
var done = false
var canvas = CreateSurface(ScreenWidth,ScreenHeight,transparent);
var amount = 0
var samount = 0
while(!done){
	if(dir<=0){dir=4} // Eliminate out of range values
	if(dir>4){dir=1}
	for(i=0;i<size;i++){
		switch(dir){
			case(1):{for(j=0;j<spd;j++){dy[j]=--y;}break;}
			case(2):{for(j=0;j<spd;j++){dx[j]=++x;}break;}
			case(3):{for(j=0;j<spd;j++){dy[j]=++y;}break;}
			case(4):{for(j=0;j<spd;j++){dx[j]=--x;}break;}}
	if(x<2 || x>=ScreenWidth-1 || y<2 || y>=ScreenHeight-1){
		done = true; break;
	}
	font.drawText(0,20,"x: "+x)
	font.drawText(0,30,"y: "+y)
	if(dir==2||dir==4){
		for(k=0;k<dx.length;k++){
			canvas.setPixel(dx[k],y,white);
			amount++
	}}
	if(dir==1||dir==3){
		for(k=0;k<dy.length;k++){
			canvas.setPixel(x,dy[k],white);
			amount++
	}}
	samount++
	canvas.blit(0,0);
	font.drawText(0,0,"Current Direction: "+dir)
	font.drawText(0,10,"Current StepSize: "+size)
	font.drawText(0,40,"Pixels Drawn: "+amount)
	font.drawText(0,50,"Steps Drawn: "+samount)
	FlipScreen();
	}
	rnd = Math.round(Math.random());
	if(rnd<0.5){dir--}else{dir++}
	size = size * ((Math.random()*frnd)-(Math.random()*frnd))
	while(size < 1){size++}
	while(size > 150){size -= 80 + (Math.random() * 60)}

}
	canvas.blit(0,0);
	font.drawText(0,0,"Assignment complete.")
	FlipScreen();
	GetKey();


}