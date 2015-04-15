using System;
using Wumpus.Model.Logic;

namespace Wumpus.Model.Presistence
{
    public interface IWumpusDataAccess
    {
	    void Save(String fileName, WumpusGameLogic model);

	    WumpusGameLogic LoadGame(string fileName);

    }
}
