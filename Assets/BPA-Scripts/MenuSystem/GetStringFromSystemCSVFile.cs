using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GetStringFromSystemCSVFile : MonoBehaviour
{
    /*
     * This will let you read a string from a CSV file and place it in the menu. 
     * Menu Text should be placed in this CSV file for easy translation and spell check.
     * Other language support can be added to the CommaSeperatedValueParser class. 
     * I'll likley do that soon. The plan is that langues should go in a defined order.
     * that order should be placed at the top of the CSV file so that someone editing knows where to put which language file.
     * 
     * you can also make the CSV in a spread sheet and export later.
     * 
     */
    [Header("Read toolTip!")]
    [Tooltip("Defaults to static instance of SystemCSVDataHolder. be sure to set it up in your main scene!")]
    public SystemCSVDataHolder sourceOverride;
    public string sourceGroup="Default";
    public TextMeshProUGUI displayAsTextMeshPro;
    // Start is called before the first frame update
    void Start()
    {
        if (sourceOverride == null) sourceOverride = SystemCSVDataHolder.instance;
        if (displayAsTextMeshPro == null) displayAsTextMeshPro = this.GetComponent<TextMeshProUGUI>();
        if (displayAsTextMeshPro != null) displayAsTextMeshPro.SetText(sourceOverride.GetCSVData(sourceGroup));
    }

}
