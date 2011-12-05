using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace MVCJpgKeywords.MyClasses
{

    public class MyAppSettings{

#region strings


    private static string emailException = "Information for the web developer: for the application to be able to send e-mails " + 
                            "in the application settings the following sections should be properly configured:<br>\r" + 
                            "Section SmtpServerName should contain the alias of the Smtp Server<br>\r" + 
                            "Section FromEmailForThisApplication should contain a valid e-mail address that will be placed in the from field of the e-mail<br>\r" + 
                            "Section FromEmailUserName should contain the name of the user (only name without domain name) that has permissions to send from this e-mail<br>\r" + 
                            "Section FromEmailPassword shoud contain the password for this user<br>\r";
#endregion //strings

#region "Settings - String"

    //Email addresses who wants to receive new registration notifications
    public static string PageTitlePrefix{
        get{
            return GetStringFromAppSettings("PageTitlePrefix","");
        }
    }

    public static string AppSettingsTemplate {
        get{
            Boolean needSave = false;
            Configuration myConfiguration = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~");
            AppSettingsTemplateConfSection templSection = (AppSettingsTemplateConfSection)myConfiguration.Sections["AppSettingsTemplate"];
            if (templSection == null){
                templSection = new AppSettingsTemplateConfSection();
                myConfiguration.Sections.Add("AppSettingsTemplate", templSection);
                needSave = true;
            }


            string ausgabe  = templSection.settingKeys;
            if (string.IsNullOrEmpty(ausgabe)){ //creating AppSettingsTemplate key
                string val = "";
                string separator = "";
                foreach (string s in MyAppSettings.EnumerateAppSettingKeys()){
                    val += separator + s;
                    separator = ";";
                }

                templSection.settingKeys = val;
                ausgabe = templSection.settingKeys;
                needSave = true;
            }
            if (needSave){ 
                myConfiguration.Save();
                System.Configuration.ConfigurationManager.RefreshSection("AppSettingsTemplate");
            }
            return ausgabe;
        }
        set{
            string a = AppSettingsTemplate; //becouse Get initiates the ConfigSection if it absent
            Configuration  myConfiguration = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~");
            AppSettingsTemplateConfSection templSection = (AppSettingsTemplateConfSection)myConfiguration.Sections["AppSettingsTemplate"];
            templSection.settingKeys = value;
            myConfiguration.Save();
            System.Configuration.ConfigurationManager.RefreshSection("AppSettingsTemplate");
        }
    }


#endregion //Settings - String

#region "Settings - Boolean"

    /// <summary>
    /// Some settings and messages in the application output more detailed information, if this flag is set.
    /// </summary>
    /// <returns></returns>
    /// <remarks></remarks>
    public static Boolean IsDebugMode {
        get {
            return GetBooleanFromAppSettings("IsDebugMode", false);
        }
    }

#endregion //Settings - Boolean

#region "Settings - Integer"
    public static int maxNumberOfParticipants{
        get{
            return GetIntegerFromAppSettings("maxNumberOfParticipants", 0);
        }
    }
#endregion //Settings - Integer

#region "other types"
    public static ConferenceStatus.ConferenceStates workshopStatus{
        get{
            ConferenceStatus.ConferenceStates ausgabe = ConferenceStatus.ConferenceStates.registration_closed;
            string val = GetStringFromAppSettings("workshopStatus");
            foreach (ConferenceStatus.ConferenceStates c in Enum.GetValues(typeof(ConferenceStatus.ConferenceStates))){
                if (val == Enum.GetName(typeof(ConferenceStatus.ConferenceStates),c)) {
                    ausgabe = c;
                    break;
                }
            }
            return ausgabe;
        }
    }
#endregion //other types

#region "Procedures"
    /// <summary>
    /// Converts the value stored in ConfigurationManager.AppSettings section to boolean.
    /// If the key doesn't exist - returns standardValue from second parameter.
    /// </summary>
    /// <param name="settingName">AppSettings key name</param>
    /// <param name="standardValue">Value to use if the AppSettings key doesn't exist</param>
    /// <returns></returns>
    /// <remarks></remarks>
    protected static Boolean GetBooleanFromAppSettings(string settingName, Boolean standardValue){
        Boolean ausgabe  = standardValue;
        string settingValue = ConfigurationManager.AppSettings[settingName];
        if (!(String.IsNullOrEmpty(settingValue))){
            if (settingValue == "1" | 
                settingValue.ToLower() == "yes" | 
                settingValue.ToLower() == "ja" | 
                settingValue.ToLower() == "true"){

                ausgabe = true;
            }
            else{
                ausgabe = false;
            }
        }

        return ausgabe;
    }

    /// <summary>
    /// Converts the value stored in ConfigurationManager.AppSettings section to integer.
    /// If the key doesn't exist - returns standardValue from second parameter.
    /// </summary>
    /// <param name="settingName">AppSettings key name</param>
    /// <param name="standardValue">Value to use if the AppSettings key doesn't exist</param>
    /// <returns></returns>
    /// <remarks></remarks>
    protected static int GetIntegerFromAppSettings(string settingName, int standardValue){
        int ausgabe = standardValue;
        string settingValue= ConfigurationManager.AppSettings[settingName];
        if (!(String.IsNullOrEmpty(settingValue))){
            try
            {
                ausgabe = Int32.Parse(settingValue);
            }
            catch { }
        }
        return ausgabe;
       }

    /// <summary>
    /// returns the value stored in ConfigurationManager.AppSettings section.
    /// </summary>
    /// <param name="settingName">AppSettings key name</param>
    /// <param name="standardValue">Value to use if the AppSettings key doesn't exist.</param>
    ///returns></returns>
    /// <remarks></remarks>
    protected static string GetStringFromAppSettings(string settingName, string standardValue = "") {  
        string ausgabe = ConfigurationManager.AppSettings[settingName];
        if (ausgabe == null){
            ausgabe = standardValue;
        }
        return ausgabe;
    }

    public static List<string> EnumerateAppSettingKeys() {
        List<string> ausgabe = new List<string>();
        foreach (string k in ConfigurationManager.AppSettings.Keys){
            if (!(k == "AppSettingsTemplate")){
                ausgabe.Add(k);
            }
        }
        return ausgabe;
    }

    public static List<string> EnumerateAppSettingsTemplate() {
        List<string> ausgabe = new List<string>();
        foreach (string s in AppSettingsTemplate.Split(';')){
            ausgabe.Add(s);
        }
        return ausgabe;
    }


    public static void CheckConsistensyOfAppSettings(){
        List<string> origS = EnumerateAppSettingKeys();
        List<string> templS = EnumerateAppSettingsTemplate();

        string entriesToAdd = "";
        string separator = "";
        if (templS.Count > 0){
            separator = ";";
        }

        string alarmText = "";

        Boolean  needSave = false;

        //removing from both arrays entries that match each other
        for (int i = origS.Count - 1;i>=0; i--){
            for (int j = templS.Count - 1;j>=0;j--){ 
            if (origS[i].ToLower() == templS[j].ToLower()){
                origS.RemoveAt(i);
                templS.RemoveAt(j);
                break;
            }
            }
        }

        foreach (string s in origS){
            entriesToAdd += separator + s;
            separator = ";";
            needSave = true;
        }

        if (needSave){ 
            AppSettingsTemplate += entriesToAdd;
        }

        foreach(string s in templS){
            alarmText += s + "/r";
        }

        if (!(String.IsNullOrEmpty(alarmText))){
           alarmText = "Some AppSettings entries are missing. Please add following AppSettings entries to your web configuration, or remove them from AppSettingsTemplate section of Web.config:\r" + alarmText;
           throw new Exception(alarmText);
        }

        //analize remaining entries.
    }

#endregion //Procedures

    }

public class AppSettingsTemplateConfSection:System.Configuration.ConfigurationSection{

    [ConfigurationProperty("settingKeys", DefaultValue = "", IsRequired = true)]
    public string settingKeys{
        get{return (string)this["settingKeys"];}
        set{this["settingKeys"] = value;}
    }
}

}