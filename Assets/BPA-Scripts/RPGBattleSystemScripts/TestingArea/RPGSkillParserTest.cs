using RPG.BPA;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RPGSkillParserTest : MonoBehaviour
{
    public TextAsset skillDefinition;
    public RPGSkillParser skillParser= new RPGSkillParser();

    // Start is called before the first frame update
    void Start()
    {
        skillParser.ParseSkillsFromData(skillDefinition.text);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
