using System.Windows.Forms;

namespace nAble
{
	public class DblClickButton : Button
	{
		public DblClickButton()
		{
			SetStyle(ControlStyles.StandardClick | ControlStyles.StandardDoubleClick, true);
			//SetStyle(ControlStyles.StandardDoubleClick, true);
		}
	}

}
