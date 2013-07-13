// mousetest

function TestMouse()
{
	var done = false;
    var arrow = GetSystemArrow();
    var a_x = GetScreenWidth() / 2 - arrow.width / 2;
    var a_y = GetScreenHeight() / 2 - arrow.height / 2;
	var green = CreateColor(0, 255, 0);

    while (!done) {
        var x = GetMouseX();
        var y = GetMouseY();

        var dx = x - (a_x + arrow.width / 2);
        var dy = y - (a_y + arrow.height / 2);

        var angle = Math.atan2(dy, dx);

		Point(x, y, green);
        arrow.rotateBlit(a_x, a_y, angle);
        FlipScreen();

		while (AreKeysLeft()) {
			if (GetKey() == KEY_ENTER) done = true;
		}
    }
}