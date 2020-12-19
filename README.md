# fakesunburst
Defanged version of sunburst backdoor base in the decompiled version from Chris Doman https://github.com/cadosecurity/MalwareAnalysis/blob/main/OrionImprovementBusinessLayer.cs 
Basically is the defanged version so you can compile it and see what sunburst will do before it tries to connect to the C2 server. Also allow to control its behavior by disabling checks, time delays, force a specific family, see the proceses that you are running that sunburt is interested in and see which ones will stop it. 

### Defanged:
  - This version has the C2 (Command & Control) code disabled. 
  - All connections will be done to 'localhost' by default. You need to specify a different host with -a parameter
  - This tool uses the same host for DNS resolution and to test C2. The real backdoor uses 'api.solarwinds.com' and '{DGA}.avsvmcloud.com however' in this tool you use the same host   you define. by default is 'localhost'
  - The only thing will do is try to connect to the host you specify. After the connection is made the connection is dropped.
 
### Uses:
  - To understand more about sunburst and properly assess the situation 
  - To test your previous and current posture of your endpoints and network controls against a sunburst simulation
  - To test if your current endpoint solution now detects it
  - To see why your NextGen AV or Preventive NextGen IA Endpoint Solution did not stop it before it was discloswed by Fireeye
  - To see why your endpoint solution allowed to fully execute this tool without stoping it at the right time
  - To see why the backdoor never executed in your environment 
  - To see what forensics artifacs, logs, traces are left in your monitoring solutions and SIEM to really understand this incident or improve your posture
  - To use it for table top exercises
  - To understand the code and match it against whats has been publicly disclosed
  - To learn more about malware
  
### How to run it:

fakesunburst.exe -h
DEFANGED-SUNBURST v1.1 ==================== ET Lownoise 2020
(-h for Help)
     Example:   fakesunburst.exe -a www.something.com
     Options:

     -a [host] Use this host as C2 test and DNS resolution. In the backdoor it
                uses 'api.solarwinds.com' and {DGA}.avsvmcloud.com however by default in
                this tool it points to 'localhost' and needs to be changed wiht this flag.
     -b Bypass businesslayerhost check
     -t Bypass file timestamp check
     -w Bypass DNS resolve check
     -s Bypass status check
     -x Bypass time delays
     -d Bypass domain check
     -r Bypass drivers/processes check
     -n Bypass C2 hostname check
     -1 Force Netbios Family
     -2 Force Implink Family
     -3 Force Atm Family
     -4 Force Ipx Family
     -5 Force InterNetwork Family
     -6 Force InternetworkV6 Family
     -7 Force Unknown Family
     -8 Force Error Family
     -p Dont print list of processes
     -i Dont print list of services
     -y Dont print list of drivers
     -m Dont print list of network/family
     -u Force scan of connfiguration
     -h This help

### Example:

fakesunburst.exe 

    [09.26.27.4962731] - DEFANGED-SUNBURST v1.1 ==================== ET Lownoise 2020
    [09.26.27.5032548] - (-h for Help)
    [09.26.27.5042238] - Will be using 'localhost' as fake C2 and test connectivity via DNS resolution
    [09.26.27.5042238] - Initializing --  Initialize()
    [09.26.27.5042238] - Trying to get Current Process Name and compare to hash of 'solarwinds.businesslayerhost'.
    [09.26.27.5072474] - Check failed sunburst not running from 'solarwinds.businesslayerhost' runnin from fakesunburst[-b to bypass]
    [09.26.27.5092411] - FAKESUNBURST EXIT 

  ---->   Sunburst checks that it is run as solarwinds.businesslayerhost.exe and because it fails the check then it stops. Lets use -b to bypass that check
  
fakesunburst.exe -b

    [09.28.18.8484282] - DEFANGED-SUNBURST v1.1 ==================== ET Lownoise 2020
    [09.28.18.8564070] - (-h for Help)
    [09.28.18.8564070] - Will be using 'localhost' as fake C2 and test connectivity via DNS resolution
    [09.28.18.8564070] - Initializing --  Initialize()
    [09.28.18.8564070] - Trying to get Current Process Name and compare to hash of 'solarwinds.businesslayerhost'.
    [09.28.18.8603683] - Check succeed sunburst running from 'solarwinds.businesslayerhost'
    [09.28.18.8603683] - Backdoor file last write/modified:12/18/2020 8:55:58 PM
    [09.28.18.8603683] - Random number of hours between 288 and 336:295. This is 12days.
    [09.28.18.8603683] - Check failed sunburst file was modified less than 295hours ago. [-t to bypass]
    [09.28.18.8603683] - FAKESUNBURST EXIT 
  
 ---->   Sunburst checks the timestamp of the file and will only execute if you run it after 12 or 14 days of the timestamp of the file. Lets use -t to bypass this check

fakesunburst.exe -b -t

    [09.30.20.9560111] - DEFANGED-SUNBURST v1.1 ==================== ET Lownoise 2020
    [09.30.20.9659540] - (-h for Help)
    [09.30.20.9659540] - Will be using 'localhost' as fake C2 and test connectivity via DNS resolution
    [09.30.20.9659540] - Initializing --  Initialize()
    [09.30.20.9659540] - Trying to get Current Process Name and compare to hash of 'solarwinds.businesslayerhost'.
    [09.30.20.9689707] - Check succeed sunburst running from 'solarwinds.businesslayerhost'
    [09.30.20.9699431] - Backdoor file last write/modified:12/18/2020 8:55:58 PM
    [09.30.20.9699431] - Random number of hours between 288 and 336:335. This is 13days.
    [09.30.20.9699431] - Check succeed sunburst file was modified more than 335hours ago. This is 13days ago.
    [09.30.20.9709672] - New Named pipe with pipe name set to appId static value: 583da945-62af-10e8-4902-a8f205c72b2e
    [09.30.20.9709672] - Backdoor Current status: New
    [09.30.20.9709672] - Check succeed backdoor status is different that status 'truncate'
    [09.30.20.9709672] - Sleeping minimum is 30mins and maximum is 120mins.
    [09.30.20.9719375] - Sleeping execution for 7108711 Milliseconds. 7108secs. 118mins. [-x to bypass]

---->   Sunburst internally has multiple delay that each one is between 30mins to 120mins. Lets bypass those with -x

fakesunburst.exe -b -t -x

    [09.32.15.4532141] - DEFANGED-SUNBURST v1.1 ==================== ET Lownoise 2020
    [09.32.15.4611618] - (-h for Help)
    [09.32.15.4611618] - Will be using 'localhost' as fake C2 and test connectivity via DNS resolution
    [09.32.15.4611618] - Initializing --  Initialize()
    [09.32.15.4611618] - Trying to get Current Process Name and compare to hash of 'solarwinds.businesslayerhost'.
    [09.32.15.4651510] - Check succeed sunburst running from 'solarwinds.businesslayerhost'
    [09.32.15.4651510] - Backdoor file last write/modified:12/18/2020 8:55:58 PM
    [09.32.15.4651510] - Random number of hours between 288 and 336:328. This is 13days.
    [09.32.15.4651510] - Check succeed sunburst file was modified more than 328hours ago. This is 13days ago.
    [09.32.15.4661486] - New Named pipe with pipe name set to appId static value: 583da945-62af-10e8-4902-a8f205c72b2e
    [09.32.15.4661486] - Backdoor Current status: New
    [09.32.15.4661486] - Check succeed backdoor status is different that status 'truncate'
    [09.32.15.4671465] - Bypassing time delays. By default this was between 30 and 120 minutes.
    [09.32.15.5070391] - Domain name is testdomain.com
    [09.32.15.5080365] - Check succeed Domain name is valid: domain.com
    [09.32.15.5080365] - Bypassing time delays. By default this was between 30 and 120 minutes.
    [09.32.15.6077983] - Device info: C8F7FFFF5FE
    [09.32.15.6087674] - Registry/MachineGuid info: C8F750Ftestomaina8bcab000-81c2-64000001d5fe
    [09.32.15.6087674] - Check succeed to create unique identificator.
    [09.32.15.6097842] - Bypassing time delays. By default this was between 30 and 120 minutes.
    [09.32.15.6097842] - Read backdoor config services status
    [09.32.15.6097842] - Almost ready to hit the fan
    [09.32.15.6128094] - Entered UpdateNotification()
    [09.32.15.6128094] - UpdateNotification is done 3times
    [09.32.15.6128094] - UpdateNotification round2
    [09.32.15.6128094] - Bypassing time delays. By default this was between 30 and 120 minutes.
    [09.32.15.6137760] - Ready to start getting system processes
    [09.32.15.6157708] - List of processes obtained
    [09.32.15.6157708] - Ready to start searching processes
             - Assembly/Process: WavesSysSvc64
             - Assembly/Process: svchost
             -...
             - Assembly/Process: svchost
             - Assembly/Process: Wireshark
    [09.32.15.6207565] - Interesting assembly found:Wireshark[-r to Bypass]    <--- NOTE THIS!!!
    [09.32.15.6207565] - SearchAssemblies in TrackProcesses() returning true
    [09.32.15.6207565] - Backdoor TrackProcesses() complete and check now returns false
    [09.32.15.6207565] - UpdateNotification() failed.
    [09.32.15.6217545] - Sunburst finished.
    [09.32.15.6217545] - FAKESUNBURST EXIT

---->   Sunburst check for specific processeses, drivers and services that  if found it end it execution. This is great way to find it tha backdoor did not ececuted completely.
        [-r ] will bypass the check but it will shouw you all hits.
        
    [09.38.35.1340475] - Checking property: PathName  GetFileName: your_awesome_endpoint.sys
    [09.38.35.1340475] - Check for special drivers failed . Backdoor ConfigTimeStamps detected last GetFileName with hash 12345678900000 [Use -r to Bypass]
    [09.38.35.1340475] - Because you are bypassing drivers/process check, the backdoor will continue. In a normal case teh backdoor stop execution.
    ...
    [09.38.35.4212812] - Backdoor CheckServerConnection() to the Internet (Actually it just checks if it can resolve)
    [09.38.35.4232837] - CheckServerConnection() failed unable to resolve: localhost [Maybe use -a host] or [-w to bypass check]
    [09.38.35.4232837] - UpdateNotification() failed.
    [09.38.35.4232837] - Sunburst finished.
    [09.38.35.4232837] - FAKESUNBURST EXIT     

---->  Sunburst check for be able to resolve the C2 hostname. Use -a www.yoursite.com  
        
    [09.44.11.6510434] - TrackProcesses() complete.
    [09.44.11.6530628] - GetStatus() return new C2 host .appsync-api.us-east-1.avsvmcloud.com
    [09.44.11.6530628] - hostName var set to: dfddgdgdfdfdfdfdf2e2.appsync-api.us-east-1.avsvmcloud.com  <--- This is the C2 hostname generated by sunburst to connect but is only for diplay purposes as it will uses www.yoursite.com 
    [09.44.11.6530628] - Backdoor is pulling the dnsRecords of C2: fakesinburst.Program+DnsRecords
    [09.44.11.6530628] - Bypassing original C2 hostname and instead will be using www.yoursite.com
    [09.44.11.6540702] - Ip address resolved for www.yoursite.com 100.200.200.200
    [09.44.11.6540702] - Address family is InterNetwork  
    [09.44.11.6540702] - Geting addresses for 1000.200.200.200 Recfakesinburst.Program+DnsRecords
    [09.44.11.6540702] - Checking IP address to predefined networks: 10.0.0.0 255.0.0.0 False Atm
    [09.44.11.6540702] - Checking IP address to predefined networks: 172.16.0.0 255.240.0.0 False Atm
    [09.44.11.6550329] - Checking IP address to predefined networks: 192.168.0.0 255.255.0.0 False Atm
    [09.44.11.6550329] - Checking IP address to predefined networks: 224.0.0.0 240.0.0.0 False Atm
    [09.44.11.6550329] - Checking IP address to predefined networks: fc00:: fe00:: False Atm
    [09.44.11.6560302] - Checking IP address to predefined networks: fec0:: ffc0:: False Atm
    [09.44.11.6560302] - Checking IP address to predefined networks: ff00:: ff00:: False Atm
    [09.44.11.6560302] - Checking IP address to predefined networks: 41.84.159.0 255.255.255.0 False Ipx
    [09.44.11.6560302] - Checking IP address to predefined networks: 74.114.24.0 255.255.248.0 False Ipx
    [09.44.11.6560302] - Checking IP address to predefined networks: 154.118.140.0 255.255.255.0 False Ipx
    [09.44.11.6560302] - Checking IP address to predefined networks: 217.163.7.0 255.255.255.0 False Ipx
    [09.44.11.6560302] - Checking IP address to predefined networks: 20.140.0.0 255.254.0.0 False ImpLink
    [09.44.11.6560302] - Checking IP address to predefined networks: 96.31.172.0 255.255.255.0 False ImpLink
    [09.44.11.6560302] - Checking IP address to predefined networks: 131.228.12.0 255.255.252.0 False ImpLink
    [09.44.11.6560302] - Checking IP address to predefined networks: 144.86.226.0 255.255.255.0 False ImpLink
    [09.44.11.6560302] - Checking IP address to predefined networks: 8.18.144.0 255.255.254.0 False NetBios
    [09.44.11.6560302] - Checking IP address to predefined networks: 18.130.0.0 255.255.0.0 True NetBios
    [09.44.11.6560302] - Checking IP address to predefined networks: 71.152.53.0 255.255.255.0 False NetBios
    [09.44.11.6560302] - Checking IP address to predefined networks: 99.79.0.0 255.255.0.0 True NetBios
    [09.44.11.6560302] - Checking IP address to predefined networks: 87.238.80.0 255.255.248.0 False NetBios
    [09.44.11.6570272] - Checking IP address to predefined networks: 199.201.117.0 255.255.255.0 False NetBios
    [09.44.11.6570272] - Checking IP address to predefined networks: 184.72.0.0 255.254.0.0 True NetBios
    [09.44.11.6570272] - AddressFamily is (-1 Netbios, -2 ImpLink, -3 Atm, -4 Ipx, -5 InterNetwork, -6 InterNetworkV6, -7 Unknown, -8 Error) : InterNetwork [-1-8 to force Family]
    [09.44.11.6570272] - Sunburst finished.
    [09.44.11.6570272] - FAKESUNBURST EXIT

---->  Read about sunburst networks and families in the different blogs. but basically is the IP resolved is compared to a list of hardcoded networks and a family is assigned. According to the family it will perform diferent actions. in some case some familieis dont do anything. So check your DNS records and see what IP sunburst provided that will tell you what actions the backdoor performed. Lets force a speciic family with -1 to -8 (-1 Netbios, -2 ImpLink, -3 Atm, -4 Ipx, -5 InterNetwork, -6 InterNetworkV6, -7 Unknown, -8 Error)


Forcing familily to Netbios (-1)

    [09.50.05.6972380] - Forcing Netbios family
    [09.50.05.6972380] - Backdoor status is NEW
    [09.50.05.6972380] - HTTPHELPER
    [09.50.06.3685098] - Done Sleeping
    THE BACKDOOR CONNECTED TO C2 SERVER www.yoursite
    THE END.=====

(Fakesunburst will continue in a loop just Control+C' and if you see "THE BACKDOOR CONNECTED TO C2 SERVER" it means that is you can now test for other scenarios as you can asume that in the real situation the machine is under the control of the attackers)

Regards,

ET
\*





