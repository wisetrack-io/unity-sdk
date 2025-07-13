using System.IO;
using System.Xml;
using UnityEditor;
using UnityEditor.Android;
using UnityEngine;

#if UNITY_ANDROID
[InitializeOnLoad]
public class AndroidManifestProcessor : IPostGenerateGradleAndroidProject
{
    public int callbackOrder => 1;

    public void OnPostGenerateGradleAndroidProject(string path)
    {
        string manifestPath = Path.Combine(path, "src/main/AndroidManifest.xml");
        
        if (File.Exists(manifestPath))
        {
            ModifyManifest(manifestPath);
        }
    }

    private void ModifyManifest(string manifestPath)
    {
        XmlDocument doc = new XmlDocument();
        doc.Load(manifestPath);

        XmlNamespaceManager namespaceManager = new XmlNamespaceManager(doc.NameTable);
        namespaceManager.AddNamespace("android", "http://schemas.android.com/apk/res/android");

        XmlNode manifestNode = doc.SelectSingleNode("/manifest");
        
        AddPermission(doc, manifestNode, namespaceManager, "android.permission.INTERNET");
        AddPermission(doc, manifestNode, namespaceManager, "android.permission.ACCESS_NETWORK_STATE");
        AddPermission(doc, manifestNode, namespaceManager, "com.google.android.gms.permission.AD_ID");

        XmlNode applicationNode = doc.SelectSingleNode("/manifest/application");
        AddMetaData(doc, applicationNode, namespaceManager, "io.sentry.auto-init", "false");

        doc.Save(manifestPath);
        Debug.Log("AndroidManifest.xml modified successfully");
    }

    private void AddPermission(XmlDocument doc, XmlNode manifestNode, XmlNamespaceManager namespaceManager, string permission)
    {
        string xpath = $"//uses-permission[@android:name='{permission}']";
        if (doc.SelectSingleNode(xpath, namespaceManager) == null)
        {
            XmlElement permissionElement = doc.CreateElement("uses-permission");
            permissionElement.SetAttribute("name", "http://schemas.android.com/apk/res/android", permission);
            manifestNode.InsertBefore(permissionElement, manifestNode.FirstChild);
        }
    }

    private void AddActivity(XmlDocument doc, XmlNode applicationNode, XmlNamespaceManager namespaceManager, string activityName)
    {
        string xpath = $"//activity[@android:name='{activityName}']";
        if (doc.SelectSingleNode(xpath, namespaceManager) == null)
        {
            XmlElement activityElement = doc.CreateElement("activity");
            activityElement.SetAttribute("name", "http://schemas.android.com/apk/res/android", activityName);
            activityElement.SetAttribute("theme", "http://schemas.android.com/apk/res/android", "@android:style/Theme.Translucent.NoTitleBar");
            activityElement.SetAttribute("exported", "http://schemas.android.com/apk/res/android", "false");
            applicationNode.AppendChild(activityElement);
        }
    }

    private void AddService(XmlDocument doc, XmlNode applicationNode, XmlNamespaceManager namespaceManager, string serviceName)
    {
        string xpath = $"//service[@android:name='{serviceName}']";
        if (doc.SelectSingleNode(xpath, namespaceManager) == null)
        {
            XmlElement serviceElement = doc.CreateElement("service");
            serviceElement.SetAttribute("name", "http://schemas.android.com/apk/res/android", serviceName);
            serviceElement.SetAttribute("exported", "http://schemas.android.com/apk/res/android", "false");
            applicationNode.AppendChild(serviceElement);
        }
    }

    private void AddMetaData(XmlDocument doc, XmlNode applicationNode, XmlNamespaceManager namespaceManager, string name, string value)
    {
        string xpath = $"//meta-data[@android:name='{name}']";
        if (doc.SelectSingleNode(xpath, namespaceManager) == null)
        {
            XmlElement metaDataElement = doc.CreateElement("meta-data");
            metaDataElement.SetAttribute("name", "http://schemas.android.com/apk/res/android", name);
            metaDataElement.SetAttribute("value", "http://schemas.android.com/apk/res/android", value);
            applicationNode.AppendChild(metaDataElement);
        }
    }
}
#endif