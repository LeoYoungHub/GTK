using System;
using System.Collections.Generic;
using System.IO.Ports;
using Gtk;
using UI = Gtk.Builder.ObjectAttribute;

namespace safeprojectname
{
	class MainWindow : Window
	{
		[UI] Label label1 = null;
		static Dictionary<string, KeyValuePair<string, string>> Commands = new Dictionary<string, KeyValuePair<string, string>>() {
		{"0",new KeyValuePair<string, string>("01 0F 00 00 00 08 01 FF BE D5","01 0F 00 00 00 08 01 00 FE 95") },
		{"1",new KeyValuePair<string, string>("01 05 00 00 FF 00 8C 3A","01 05 00 00 00 00 CD CA") },
		{"2",new KeyValuePair<string, string>("01 05 00 01 FF 00 DD FA","01 05 00 01 00 00 9C 0A") },
		{"3",new KeyValuePair<string, string>("01 05 00 02 FF 00 2D FA","01 05 00 02 00 00 6C 0A") },
		{"4",new KeyValuePair<string, string>("01 05 00 03 FF 00 7C 3A","01 05 00 03 00 00 3D CA") }
		};

		public MainWindow() : this(new Builder("MainWindow.glade")) { }

		private MainWindow(Builder builder) : base(builder.GetObject("MainWindow").Handle)
		{
			builder.Autoconnect(this);

			GetSerialPort();

			DeleteEvent += Window_DeleteEvent;
		}
		static Dictionary<string, string> ports = new Dictionary<string, string>();

		void GetSerialPort()
		{
			Console.WriteLine("发现串口：");
			var names = SerialPort.GetPortNames();
			int count = 1;
			foreach (var item in names)
			{
				ports.Add(count.ToString(), item);
				Console.WriteLine($"{count.ToString()}：{item}");
				count++;
			}
			Console.WriteLine("输入要打开的串口编号：" + ports.Keys.Join(","));

			string portName = ports["2"];

			_SerialPort = new SerialPort(portName, 9600, Parity.None, 8, StopBits.One);
			_SerialPort.Open();
		}

		private void Window_DeleteEvent(object sender, DeleteEventArgs a)
		{
			Application.Quit();
		}
		SerialPort _SerialPort = null;

		private void on_btnOne_clicked(object sender, EventArgs a)
		{
			Button button = sender as Button;
			button.Label = button.TooltipMarkup;
		}

		private void on_togglebutton1_toggled(object sender, EventArgs a)
		{
			ToggleButton toggle = sender as ToggleButton;
			if (toggle.Active)
			{
				SendCommand(Commands[toggle.TooltipMarkup].Key);
			}
			else
			{
				SendCommand(Commands[toggle.TooltipMarkup].Value);
			}
		}

		void SendCommand(string command)
		{
			Console.WriteLine(command);
			var buf = command.ToHex();
			if (_SerialPort != null && _SerialPort.IsOpen)
				_SerialPort.Write(buf, 0, buf.Length);
		}
	}
}
