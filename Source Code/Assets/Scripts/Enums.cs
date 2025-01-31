using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enums
{
    public enum ClueType
    {
        Discovery,
        Location
    }
    
    public enum ClueName
    {
        ReligiousMark,
        GeorgeTyndale,
        LostThing,
        VoynichManuscript,
        HouseNumber,
        Gathering,
        Darkness,
        NewPath,
        Valley,
        Sewers,
        Block,
        Subway,
    }

    public enum StoryType
    {
        FirstEnterGame,
        GoFindFirstMark,
        RecreateTheEvent,
        NameLeadToCamp,
        ManuscriptLeadToNewMark,
        FirstArriveBlock,
        MarkLeadToSewer,
        GoCheckHouseNumber,
        FirstArriveSubway,
        CrowdsShowNewPlan,
        FirstArriveSewer,
        TheLastBattle,
    }
}
