using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

// Manages interactions for all party members.
public class PartyController : MonoBehaviour
{
    /* 
     * -------------------------------------------------------------------------
     * MARK: ACCESSOR VARIABLES
     * -------------------------------------------------------------------------
     */

    private UnitController unitController;
    public UnitController UnitController
    {
    	get
    	{
    		return unitController;
    	}
    }

    private PartyMemberController[] memberControllers;

    /* 
     * -------------------------------------------------------------------------
     * MARK: LIFECYCLE FUNCTIONS
     * -------------------------------------------------------------------------
     */

    void Awake()
    {
        unitController = GameObject.FindGameObjectWithTag("Units").GetComponent<UnitController>();
        memberControllers = GameObject.FindGameObjectsWithTag("Party Member")
            .Select((member) => member.GetComponent<PartyMemberController>())
            .Cast<PartyMemberController>()
            .ToArray();
    }

    void Start()
    {
        if (unitController.LoadedUnitData != null) {
            LoadSavedData(unitController.LoadedUnitData.partyMembers);
        }
    }

    /* 
     * -------------------------------------------------------------------------
     * MARK: PERSISTENT DATA FUNCTIONS
     * -------------------------------------------------------------------------
     */

    // Load previously saved data for all party members, if they exist.
    void LoadSavedData(List<UnitInfo> partyMemberInfoList)
    {
        if (partyMemberInfoList == null) {
            return;
        }

        foreach (UnitInfo partyMemberInfo in partyMemberInfoList) {
            foreach (PartyMemberController memberController in memberControllers) {
                if (memberController.Name == partyMemberInfo.name) {
                    memberController.LoadSavedData(partyMemberInfo);
                }
            }
        }
    }

    // Save data for all party members.
    public List<UnitInfo> SaveData()
    {
        List<UnitInfo> partyMemberInfoList = new List<UnitInfo>();
        foreach (PartyMemberController memberController in memberControllers) {
            UnitInfo partyMemberInfo = memberController.SaveData();
            partyMemberInfoList.Add(partyMemberInfo);
        }
        return partyMemberInfoList;
    }

    /* 
     * -------------------------------------------------------------------------
     * MARK: TURN MANAGEMENT FUNCTIONS
     * -------------------------------------------------------------------------
     */

    // Checks if all party members are tapped.
    public bool IsPartyTapped()
    {
        return memberControllers.All(memberController => memberController.Tapped);
    }

    // Untap all party members.
    public void UntapParty()
    {
        foreach (PartyMemberController memberController in memberControllers) {
            memberController.UntapUnit();
        }
    }
}
