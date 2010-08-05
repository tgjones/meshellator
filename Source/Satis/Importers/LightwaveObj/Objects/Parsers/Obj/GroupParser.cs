namespace Satis.Importers.LightwaveObj.Objects.Parsers.Obj
{
	public class GroupParser : LineParser
	{
		private Group _newGroup;

		public override void Parse()
		{
			_newGroup = new Group(Words[1]);
		}

		public override void IncorporateResults(WavefrontObject wavefrontObject)
		{
			if (wavefrontObject.CurrentGroup != null)
				wavefrontObject.CurrentGroup.Pack();

			wavefrontObject.Groups.Add(_newGroup);
			wavefrontObject.GroupsDirectAccess.Add(_newGroup.Name, _newGroup);

			wavefrontObject.CurrentGroup = _newGroup;
		}
	}
}