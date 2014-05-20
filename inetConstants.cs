using System;


namespace kioskplus
{
   public static class inetConstants
    {
       public const String icsRegKey = "Software\\Microsoft\\Windows\\CurrentVersion\\kioskplus";

       // alle Registry-ValueNamen
       public  const String icsAbbruch = "abbruch";
       public const String icsIgnore = "ignore";

       public const String icsDauerIgnore = "dauerignore";
       public const String icsEinPwd = "einpwd";
       public const String icsImpPfad = "imppfad";

       public const String icsImpPrs = "impprs";
       public const String icsKombi = "kombi";

       public const String icsMPCom = "COM";
       public const String icsMPMf = "mf";
       public const String icsUPD = "upd";
       public const String icsKioskID = "kp";
       public const String icsKPTime = "kptime";

       public const String icsVerzMP = "MP";
       public const String icsVerzIE = "AMI";
       public const String icsVerzNe = "NOACCESS";
       public const String icsVerzKey = "dirnet";
       public const String icsVerzBill = "Bll";

       // Internet-Explorer Kennzeichen Browser.dll

       public const String icsErlaubt = "BE4ZK";
       public const String icsSex = "2A7B4";
       public const String icsAndere = "2A2E7";
       public const String icsRas = "2C3DA5";
       public const String icsFlag = "AC7B3";
       public const String icsErlaubtFlag = "U112EF2";

       // Münzprüfer 
       public const String icsMPArt = "mpart";

       // Language
       public const String icsLanguage = "lang";

       public const String icsSperrSeite = "http://inetplus.de";
       public const String icsBlockSeite = "http://inetplus.de";
       public const String icsCloseFiles = icsRegKey + "\\Files";

       public const String icsMutex = "Global\\matrixKhg23"; // Global\\

       // Constant
       public const String icsStandardPassword = "inetplus";

       // Bill Validator
       public const String icsBillArt = "BSelect";
       public const String icsBillMf = "billMf";

       // NoNet Files
       public const String	isFileNameIni="inetini.dat";
       public const String	isFileNameRK="inetrk.dat";
       public const String	isFileNameUms = "inetums.dat";
       public const String  isFileNameUmsKonto = "inetkto.dat";
       public const String  isFileNameAffili = "inetaff.dat";

       // Constant
       public const String isDateFormat = "yyyy-MM-dd";
       public const String isTimeFormat = "HH:mm:ss";

       public const String SYSTEMROOT = "systemroot";

       //
       public const String icsAgentName = "AgentAnim";
       public const String icsAgentDeactive = "inetagent";


       public static readonly String isSystemDirectory = Environment.GetFolderPath(Environment.SpecialFolder.System);
       public static readonly String isWindowsDirectory = Environment.GetEnvironmentVariable(SYSTEMROOT);
       

   }
}
