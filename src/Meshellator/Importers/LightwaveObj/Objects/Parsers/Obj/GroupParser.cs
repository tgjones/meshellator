namespace Meshellator.Importers.LightwaveObj.Objects.Parsers.Obj
{
	public class GroupParser : LineParser
	{
		private Group _newGroup;

		public override void Parse()
		{
			// Name is optional, apparently.
			string name = (Words.Length > 1) ? Words[1] : null;
			_newGroup = new Group(name);
		}

		public override void IncorporateResults(WavefrontObject wavefrontObject)
		{
			if (wavefrontObject.CurrentGroup != null)
				wavefrontObject.CurrentGroup.Pack();

			wavefrontObject.Groups.Add(_newGroup);

			wavefrontObject.CurrentGroup = _newGroup;
		}
	}
}