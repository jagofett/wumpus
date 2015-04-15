using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wumpus.Model.Logic;

namespace Wumpus.Presistence
{
    public interface IWumpusDataAccess
    {
	    void Save(String fileName, WumpusGameLogic model);

	    WumpusGameLogic LoadGame(string fileName);

    }
}
