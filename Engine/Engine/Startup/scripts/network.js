// networking test

function TestNetwork()
{
    var done = false;
    var name = GetLocalName();
    var addr = GetLocalAddress();

    var socket = false;

    while (!done) {
        sys_font.drawText(0, 0, "Connection Settings:");
        sys_font.drawText(0, 16, "Host Name: " + name);
        sys_font.drawText(0, 32, "Host Address: " + addr);
        sys_font.drawText(0, 48, "Looking for a connection on local (port 4040)");

        if (socket === false) {
            socket = OpenAddress(addr, 4040);
            if (socket) { socket.write(CreateByteArrayFromString("Test")); }
        }

        FlipScreen();

		while (AreKeysLeft()) {
		    if (GetKey() == KEY_ENTER) done = true;
		}
    }
}