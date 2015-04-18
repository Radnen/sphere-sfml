using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Jurassic;
using Jurassic.Library;
using SFML.Graphics;
using SFML.System;


namespace Engine.Objects
{
    /// <summary>
    /// Represents a TCP socket
    /// </summary>
    class SocketInstance : ObjectInstance
    {
        private string _address = "";
        private int _port;
        private byte[] _buffer = new byte[512];
        private TcpClient _client;
        private NetworkStream _stream;

        private static TcpListener _listener;
        private static string _myAddress;

        /// <summary>
        /// Opens a TCP client and returns it to Sphere.
        /// </summary>
        /// <param name="parent">The Parent Script Engine</param>
        /// <param name="address">IPv4 Address</param>
        /// <param name="port">The port top open</param>
        public SocketInstance(ScriptEngine parent, string address, int port)
            : base(parent.Object.InstancePrototype)
        {
            PopulateFunctions();
            _address = address;
            _port = port;

            IPEndPoint endpoint = new IPEndPoint(IPAddress.Parse(address), port);

            _client = new TcpClient();
            _client.Connect(endpoint);
            _stream = _client.GetStream();
        }

        /// <summary>
        /// Wraps a TCP Client and returns it to Sphere.
        /// </summary>
        /// <param name="parent">The Parent Script Engine</param>
        /// <param name="address">IPv4 Address</param>
        /// <param name="port">The port top open</param>
        public SocketInstance(ScriptEngine parent, string address, int port, TcpClient client)
            : base(parent.Object.InstancePrototype)
        {
            PopulateFunctions();
            _address = address;
            _port = port;
            _client = client;
            _stream = _client.GetStream();
        }

        /// <summary>
        /// Writes data to a TCP socket.
        /// </summary>
        /// <param name="array"></param>
        [JSFunction(Name = "write")]
        public void Write(ByteArrayInstance array)
        {
            _stream.Write(array.GetBytes(), 0, array.GetSize());
        }

        /// <summary>
        /// Reads data to a TCP socket.
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        [JSFunction(Name = "read")]
        public ByteArrayInstance Read(int size)
        {
            return new ByteArrayInstance(Engine, _buffer);
        }

        /// <summary>
        /// Get the pending read size.
        /// </summary>
        /// <returns></returns>
        [JSFunction(Name = "getPendingReadSize")]
        public int GetPendingReadSize()
        {
            if (!_client.Connected) return 0;
            if (!_stream.DataAvailable) return 0;
            return _stream.Read(_buffer, 0, 512);
        }

        /// <summary>
        /// Gets if the connection is connected.
        /// </summary>
        /// <returns></returns>
        [JSFunction(Name = "isConnected")]
        public bool IsConnected()
        {
            //return _socket.Connected;
            return _client.Connected;
        }

        /// <summary>
        /// Stops and closes the TCP listener.
        /// </summary>
        [JSFunction(Name = "close")]
        public void Close()
        {
            //_socket.Close();
            _client.Close();
        }

        public static SocketInstance ListenOnPort(int port, [DefaultParameterValue("127.0.0.1")] string address = "127.0.0.1")
        {
            if (_listener == null || address != _myAddress)
            {
                _myAddress = address;
                _listener = new TcpListener(IPAddress.Parse(address), port);
                _listener.Start();
            }

            if (_listener.Pending())
            {
                return new SocketInstance(Program._engine, address, port, _listener.AcceptTcpClient());
            }
            else return null;
        }
    }
}
