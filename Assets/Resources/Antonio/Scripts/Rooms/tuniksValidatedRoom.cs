using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tuniksValidatedRoom : Room{
    public bool hasUpExit;
    public bool hasDownExit;
    public bool hasLeftExit;
    public bool hasRightExit;

    public bool hasLeftToUpPath;
    public bool hasLeftToDownPath;
    public bool hasLeftToRightPath;
    public bool hasRightToUpPath;
    public bool hasRightToDownPath;
    public bool hasUpToDownPath;

    public bool MeetsConstraint(ExitConstraint requiredExits){
        if (requiredExits.upExitRequired && !hasUpExit)
            return false;

        if (requiredExits.downExitRequired && !hasDownExit)
            return false;

        if (requiredExits.leftExitRequired && !hasLeftExit)
            return false;

        if (requiredExits.rightExitRequired && !hasRightExit)
            return false;

        if (requiredExits.leftExitRequired && requiredExits.upExitRequired && !hasLeftToUpPath)
            return false;

        if (requiredExits.leftExitRequired && requiredExits.downExitRequired && !hasLeftToDownPath)
            return false;

        if (requiredExits.leftExitRequired && requiredExits.rightExitRequired && !hasLeftToRightPath)
            return false;

        if (requiredExits.rightExitRequired && requiredExits.upExitRequired && !hasRightToUpPath)
            return false;

        if (requiredExits.rightExitRequired && requiredExits.downExitRequired && !hasRightToDownPath)
            return false;

        if (requiredExits.upExitRequired && requiredExits.downExitRequired && !hasUpToDownPath)
            return false;


        return true;
    }
}
