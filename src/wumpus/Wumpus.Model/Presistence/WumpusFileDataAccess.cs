using System;
using System.IO;
using System.Xml.Serialization;
using Wumpus.Model.Logic;


namespace Wumpus.Model.Presistence
{
	public class WumpusFileDataAccess : IWumpusDataAccess
	{
		public void Save(string fileName, WumpusGameLogic model)
		{
			var outFile = File.Create(fileName);
			var formatter = new XmlSerializer(model.GetType());
			formatter.Serialize(outFile, model);
		}

		public WumpusGameLogic LoadGame(string fileName)
		{
			throw new NotImplementedException();
		}
	}
}
