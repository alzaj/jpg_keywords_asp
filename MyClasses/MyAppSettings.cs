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
    public static readonly  String PageTitlePrefix{
        get{
            return GetStringFromAppSettings("PageTitlePrefix","");
        }
    }

    public static string AppSettingsTemplate {
        get{
            Boolean needSave = false
            Configuration myConfiguration = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~");
            AppSettingsTemplateConfSection templSection = myConfiguration.Sections("AppSettingsTemplate");
            If templSection Is Nothing Then
                templSection = New AppSettingsTemplateConfSection
                myConfiguration.Sections.Add("AppSettingsTemplate", templSection)
                needSave = True
            End If
            Dim ausgabe As String = templSection.settingKeys
            If String.IsNullOrEmpty(ausgabe) Then 'creating AppSettingsTemplate key
                Dim val As String = ""
                Dim separator As String = ""
                For Each s As String In MyAppSettings.EnumerateAppSettingKeys
                    val += separator + s
                    separator = ";"
                Next

                templSection.settingKeys = val
                ausgabe = templSection.settingKeys
                needSave = True
            End If
            If needSave Then
                myConfiguration.Save()
                System.Configuration.ConfigurationManager.RefreshSection("AppSettingsTemplate")
            End If
            return ausgabe
        }
        set{
            Dim a As String = AppSettingsTemplate 'becouse Get initiates the ConfigSection if it absent
            Dim myConfiguration As Configuration = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~")
            Dim templSection As AppSettingsTemplateConfSection = myConfiguration.Sections("AppSettingsTemplate")
            templSection.settingKeys = value
            myConfiguration.Save()
            System.Configuration.ConfigurationManager.RefreshSection("AppSettingsTemplate")
        }
    }


#endregion //Settings - String

#Region "Settings - Boolean"

    ''' <summary>
    ''' Some settings and messages in the application output more detailed information, if this flag is set.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    public static readonly Property IsDebugMode() As Boolean
        Get
            return GetBooleanFromAppSettings("IsDebugMode", False)
        End Get
    End Property

    ''' <summary>
    ''' In the last wizard step the user will be asked to confirm his agreement about saving his data in the database.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    public static readonly Property AskUserForAgreement() As Boolean
        Get
            return GetBooleanFromAppSettings("AskUserForAgreement", False)
        End Get
    End Property

    public static readonly Property IsThisAppFeldmaessig() As Boolean
        Get
            return GetBooleanFromAppSettings("IsThisAppFeldmaessig", False)
        End Get
    End Property

#End Region 'Settings - Boolean

#Region "Settings - Integer"
    public static readonly Property maxNumberOfParticipants As Integer
        Get
            return GetIntegerFromAppSettings("maxNumberOfParticipants", 0)
        End Get
    End Property
#End Region 'Settings - Integer

#Region "other types"
    public static readonly Property workshopStatus As ConferenceStatus.ConferenceStates
        Get
            Dim ausgabe As ConferenceStatus.ConferenceStates = ConferenceStatus.ConferenceStates.registration_closed
            Dim val As String = GetStringFromAppSettings("workshopStatus")
            For Each c As ConferenceStatus.ConferenceStates In [Enum].GetValues(GetType(ConferenceStatus.ConferenceStates))
                If val = [Enum].GetName(GetType(ConferenceStatus.ConferenceStates), c) Then
                    ausgabe = c
                    Exit For
                End If
            Next
            return ausgabe
        End Get
    End Property
#End Region 'other types

#Region "Procedures"
    ''' <summary>
    ''' Converts the value stored in ConfigurationManager.AppSettings section to boolean.
    ''' If the key doesn't exist - returns standardValue from second parameter.
    ''' </summary>
    ''' <param name="settingName">AppSettings key name</param>
    ''' <param name="standardValue">Value to use if the AppSettings key doesn't exist</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Protected static Function GetBooleanFromAppSettings(ByVal settingName As String, ByVal standardValue As Boolean) As Boolean
        Dim ausgabe As Boolean = standardValue
        Dim settingValue As String = ConfigurationManager.AppSettings(settingName)
        If Not String.IsNullOrEmpty(settingValue) Then
            If settingValue = "1" Or _
               settingValue.ToLower = "yes" Or _
               settingValue.ToLower = "ja" Or _
               settingValue.ToLower = "true" Then

                ausgabe = True
            Else
                ausgabe = False
            End If
        End If

        return ausgabe
    End Function

    ''' <summary>
    ''' Converts the value stored in ConfigurationManager.AppSettings section to integer.
    ''' If the key doesn't exist - returns standardValue from second parameter.
    ''' </summary>
    ''' <param name="settingName">AppSettings key name</param>
    ''' <param name="standardValue">Value to use if the AppSettings key doesn't exist</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Protected static Function GetIntegerFromAppSettings(ByVal settingName As String, ByVal standardValue As Integer) As Integer
        Dim ausgabe As Integer = standardValue
        Dim settingValue As String = ConfigurationManager.AppSettings(settingName)
        If Not String.IsNullOrEmpty(settingValue) Then
            Try
                ausgabe = CType(settingValue, Integer)
            Catch ex As Exception
            End Try
        End If
        return ausgabe
    End Function

    ''' <summary>
    ''' returns the value stored in ConfigurationManager.AppSettings section.
    ''' </summary>
    ''' <param name="settingName">AppSettings key name</param>
    ''' <param name="standardValue">Value to use if the AppSettings key doesn't exist.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Protected static Function GetStringFromAppSettings(ByVal settingName As String, Optional ByVal standardValue As String = "") As String
        Dim ausgabe As String = ConfigurationManager.AppSettings(settingName)
        If ausgabe Is Nothing Then ausgabe = standardValue
        return ausgabe
    End Function

    public static Function EnumerateAppSettingKeys() As List(Of String)
        Dim ausgabe As New List(Of String)
        For Each k As String In ConfigurationManager.AppSettings.Keys
            If Not k = "AppSettingsTemplate" Then
                ausgabe.Add(k)
            End If
        Next
        return ausgabe
    End Function

    public static Function EnumerateAppSettingsTemplate() As List(Of String)
        Dim ausgabe As New List(Of String)
        For Each s As String In AppSettingsTemplate.Split(";")
            ausgabe.Add(s)
        Next
        return ausgabe
    End Function

    public static Sub CheckConsistensyOfAppSettings()
        Dim origS As List(Of String) = EnumerateAppSettingKeys()
        Dim templS As List(Of String) = EnumerateAppSettingsTemplate()

        Dim entriesToAdd As String = ""
        Dim separator As String = ""
        If templS.Count > 0 Then separator = ";"

        Dim alarmText As String = ""

        Dim needSave As Boolean = False

        'removing from both arrays entries that match each other
        For i As Integer = origS.Count - 1 To 0 Step -1
            For j As Integer = templS.Count - 1 To 0 Step -1
                If origS(i).ToLower = templS(j).ToLower Then
                    origS.RemoveAt(i)
                    templS.RemoveAt(j)
                    Exit For
                End If
            Next
        Next

        For Each s As String In origS
            entriesToAdd += separator + s
            separator = ";"
            needSave = True
        Next

        If needSave Then
            AppSettingsTemplate += entriesToAdd
        End If

        For Each s As String In templS
            alarmText += s + vbCrLf
        Next

        If Not String.IsNullOrEmpty(alarmText) Then
            alarmText = "Some AppSettings entries are missing. Please add following AppSettings entries to your web configuration, or remove them from AppSettingsTemplate section of Web.config:" + vbCrLf + alarmText
            Throw New Exception(alarmText)
        End If

        'analize remaining entries.
    End Sub

#End Region 'Procedures

    }

public class AppSettingsTemplateConfSection:System.Configuration.ConfigurationSection{

    [ConfigurationProperty("settingKeys", DefaultValue:="", IsRequired:=True)]
    public string settingKeys{
        get{return CType(Me("settingKeys"), String);}
        set{Me("settingKeys") = value;}
    }
}

}