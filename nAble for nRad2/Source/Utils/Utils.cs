namespace nAble.Utils
{
	static class CommonUtils
	{
		static public bool ProgramNumberIsValid(int programNumber)
		{
			return programNumber >= 0 && programNumber <= 7;
		}
	}
}
