﻿'------------------------------------------------------------------------------
' <auto-generated>
'     Dieser Code wurde von einem Tool generiert.
'     Laufzeitversion:4.0.30319.239
'
'     Änderungen an dieser Datei können falsches Verhalten verursachen und gehen verloren, wenn
'     der Code erneut generiert wird.
' </auto-generated>
'------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Imports System

Namespace My.Resources
    
    'Diese Klasse wurde von der StronglyTypedResourceBuilder automatisch generiert
    '-Klasse über ein Tool wie ResGen oder Visual Studio automatisch generiert.
    'Um einen Member hinzuzufügen oder zu entfernen, bearbeiten Sie die .ResX-Datei und führen dann ResGen
    'mit der /str-Option erneut aus, oder Sie erstellen Ihr VS-Projekt neu.
    '''<summary>
    '''  Eine stark typisierte Ressourcenklasse zum Suchen von lokalisierten Zeichenfolgen usw.
    '''</summary>
    <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0"),  _
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     Global.System.Runtime.CompilerServices.CompilerGeneratedAttribute(),  _
     Global.Microsoft.VisualBasic.HideModuleNameAttribute()>  _
    Friend Module Resources
        
        Private resourceMan As Global.System.Resources.ResourceManager
        
        Private resourceCulture As Global.System.Globalization.CultureInfo
        
        '''<summary>
        '''  Gibt die zwischengespeicherte ResourceManager-Instanz zurück, die von dieser Klasse verwendet wird.
        '''</summary>
        <Global.System.ComponentModel.EditorBrowsableAttribute(Global.System.ComponentModel.EditorBrowsableState.Advanced)>  _
        Friend ReadOnly Property ResourceManager() As Global.System.Resources.ResourceManager
            Get
                If Object.ReferenceEquals(resourceMan, Nothing) Then
                    Dim temp As Global.System.Resources.ResourceManager = New Global.System.Resources.ResourceManager("FloseCode.UpdateLib.Resources", GetType(Resources).Assembly)
                    resourceMan = temp
                End If
                Return resourceMan
            End Get
        End Property
        
        '''<summary>
        '''  Überschreibt die CurrentUICulture-Eigenschaft des aktuellen Threads für alle
        '''  Ressourcenzuordnungen, die diese stark typisierte Ressourcenklasse verwenden.
        '''</summary>
        <Global.System.ComponentModel.EditorBrowsableAttribute(Global.System.ComponentModel.EditorBrowsableState.Advanced)>  _
        Friend Property Culture() As Global.System.Globalization.CultureInfo
            Get
                Return resourceCulture
            End Get
            Set
                resourceCulture = value
            End Set
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die 1
        '''SprachenName=Boarisch
        '''msgUpdateBereitsVorhanden=Es gibt scho a Update!{0}Zum Installiern, muasst as {1} Programm neistartn.
        '''Update=Neie Version vom {0} Programm
        '''msgUpdateVorhanden=Es gibt a Update auf Version {0}.{1}Mogst des jetz obalon?
        '''msgUpdateInstallierenAdmin=Sie hom grod a Update obaglon.\n\nStartens as {0} Programm ois Admin ums Update zum installiern.
        '''msgKeinUpdate=Koa neie Version do
        '''msgFehlerUpdateSuchen=Fella beim Updatesuacha: {0}
        '''lblAktuelleDatei=Datei: {0}
        '''UpdateFertigstellen=Ferti [Rest der Zeichenfolge wurde abgeschnitten]&quot;; ähnelt.
        '''</summary>
        Friend ReadOnly Property Bavarian() As String
            Get
                Return ResourceManager.GetString("Bavarian", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die 1
        '''SprachenName=汉语
        '''msgUpdateBereitsVorhanden=你已下载了的更新。{0}重新启动{1}来安装它。
        '''Update={0}更新
        '''msgUpdateVorhanden=有一个更新版本{0}{1}你想现在就下载吗？
        '''msgUpdateInstallierenAdmin=您已下载了一个更新，但是你还没有安装了它。\n\n启动{0}以管理员身份来安装更新。
        '''msgKeinUpdate=现在没有更新
        '''msgFehlerUpdateSuchen=出错搜索更新的时候：{0}
        '''lblAktuelleDatei=当前文件：{0}
        '''UpdateFertigstellen=完成制订... ...
        '''msgUpdateErfolgreich=成功了下载更新！{0}若要安装此更新，您必须重新启动{1}{0}现在重新启动？
        '''msgFehlerUpdate=误差，更新的时候：{0}
        '''FehlerAusführen=尝试执行时发生错误&apos;{0}&apos;：\n\n{1}
        '''MonoUpdateHinweis=如果更新不启动，请运行以下命令：\n\n{0}
        '''UpdateHistory=更新的历 [Rest der Zeichenfolge wurde abgeschnitten]&quot;; ähnelt.
        '''</summary>
        Friend ReadOnly Property Chinese() As String
            Get
                Return ResourceManager.GetString("Chinese", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die 1
        '''SprachenName=Nederlands
        '''msgUpdateBereitsVorhanden=Er is een update beschikbaar!{0}Start {1} om het te installeren.
        '''Update={0} Update
        '''msgUpdateVorhanden=Update versie {0} is beschikbaar.{1}Wilt u het downloaden?
        '''msgUpdateInstallierenAdmin=U hebt een update gedownload maar nog niet geïnstalleerd.\n\n Start {0} als administrator om het te installeren.
        '''msgKeinUpdate=Geen updates beschikbaar
        '''msgFehlerUpdateSuchen=Fout bij het zoeken naar updates: {0}
        '''lblAktuelleDatei=Huidig bestand: {0}
        '''UpdateFertigst [Rest der Zeichenfolge wurde abgeschnitten]&quot;; ähnelt.
        '''</summary>
        Friend ReadOnly Property Dutch() As String
            Get
                Return ResourceManager.GetString("Dutch", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die 1
        '''SprachenName=English
        '''msgUpdateBereitsVorhanden=You have already downloaded an update.{0} Restart {1} to install it.
        '''Update={0} update
        '''msgUpdateVorhanden=Update to version {0} is available.{1}Do you want to download it now?
        '''msgUpdateInstallierenAdmin=You have downloaded an update but have not installed it yet.\n\n Start {0} as administrator to install the update.
        '''msgKeinUpdate=No update available
        '''msgFehlerUpdateSuchen=Error while searching updates: {0}
        '''lblAktuelleDatei=Current file: {0}
        '''UpdateFert [Rest der Zeichenfolge wurde abgeschnitten]&quot;; ähnelt.
        '''</summary>
        Friend ReadOnly Property English() As String
            Get
                Return ResourceManager.GetString("English", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die 1
        '''SprachenName=Français
        '''msgUpdateBereitsVorhanden=Il y a déjà une mise à jour.{0} Redémarrez {1} pour l&apos;installer.
        '''Update=Mise à jour de {0}
        '''msgUpdateVorhanden=Une mise à jour à version {0} est disponible.{1}Voulez vous la télécharger maintenant?
        '''msgUpdateInstallierenAdmin=Vous avez téléchargé une mise à jour.\n\nLance le logiciel {0} comme administrateur pour la installer.
        '''msgKeinUpdate=Il n&apos;y a pas de mise à jour.
        '''msgFehlerUpdateSuchen=Erreur à chercher une mise à jour: {0}
        '''lblAktuelleDatei=Fichie [Rest der Zeichenfolge wurde abgeschnitten]&quot;; ähnelt.
        '''</summary>
        Friend ReadOnly Property French() As String
            Get
                Return ResourceManager.GetString("French", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die 1
        '''SprachenName=Deutsch
        '''msgUpdateBereitsVorhanden=Es ist bereits ein Update vorhanden!{0}Starten Sie {1} neu, um es zu installieren.
        '''Update={0} Update
        '''msgUpdateVorhanden=Ein Update auf Version {0} ist vorhanden.{1}Wollen Sie dieses jetzt herunterladen?
        '''msgUpdateInstallierenAdmin=Sie haben ein Update heruntergeladen aber noch nicht installiert.\n\nStarten Sie das {0} Programm als Administrator um das Update zu installieren.
        '''msgKeinUpdate=Kein Update vorhanden
        '''msgFehlerUpdateSuchen=Fehler beim Updatesuc [Rest der Zeichenfolge wurde abgeschnitten]&quot;; ähnelt.
        '''</summary>
        Friend ReadOnly Property German() As String
            Get
                Return ResourceManager.GetString("German", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die 1
        '''SprachenName=Polski
        '''msgUpdateBereitsVorhanden=Już pobrałeś aktualizacje!{0}Uruchom ponownie {1}, żeby się zainstalowała.
        '''Update={0} Aktualizacja
        '''msgUpdateVorhanden=Aktualizacja do wersji {0} istnieje.{1}Chcesz ją pobrać teraz?
        '''msgUpdateInstallierenAdmin=Pobrałeś aktualizacje, ale jej jeszcze nie wgrałeś.\n\nUruchom program {0} jako Administrator żeby zainstalować aktualizacje.
        '''msgKeinUpdate=Brak Aktualizacji
        '''msgFehlerUpdateSuchen=Błąd wyszukiwania aktualizacji: {0}
        '''lblAktuelleDatei=Aktualny plik:  [Rest der Zeichenfolge wurde abgeschnitten]&quot;; ähnelt.
        '''</summary>
        Friend ReadOnly Property Polish() As String
            Get
                Return ResourceManager.GetString("Polish", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die 1
        '''SprachenName=Português
        '''msgUpdateBereitsVorhanden=Você já baixou uma atualização.{0}Reinicie {1} para instalá-la.
        '''Update={0} atualização
        '''msgUpdateVorhanden=Atualização para a versão {0} está disponível.{1}Você quer baixá-la agora?
        '''msgUpdateInstallierenAdmin=Você baixou uma atualização mas ela ainda não foi instalada.\n\n Inicie {0} como administrador para instalar a atualização.
        '''msgKeinUpdate=Nenhuma atualização disponível.
        '''msgFehlerUpdateSuchen=Erro durante a procura por atualizações: {0}
        '''lblAktue [Rest der Zeichenfolge wurde abgeschnitten]&quot;; ähnelt.
        '''</summary>
        Friend ReadOnly Property Portuguese() As String
            Get
                Return ResourceManager.GetString("Portuguese", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Sucht eine lokalisierte Zeichenfolge, die 1
        '''SprachenName=Español
        '''msgUpdateBereitsVorhanden=Ya existe una actualización!{0}Reiniciar {1} para instalar.
        '''Update={0} actualización
        '''msgUpdateVorhanden=Una actualización a la versión {0} existe.{1}Desea bajarla?
        '''msgUpdateInstallierenAdmin=Ha bajado una actualización pero no esta instalada.\n\nInicie el programa de administrador para instalar la actualización.
        '''msgKeinUpdate=No hay actualización
        '''msgFehlerUpdateSuchen=Fallo en la búsqueda de actualización: {0}
        '''lblAktuelleDatei=Archivo actual: {0}
        '''Upd [Rest der Zeichenfolge wurde abgeschnitten]&quot;; ähnelt.
        '''</summary>
        Friend ReadOnly Property Spanish() As String
            Get
                Return ResourceManager.GetString("Spanish", resourceCulture)
            End Get
        End Property
    End Module
End Namespace
