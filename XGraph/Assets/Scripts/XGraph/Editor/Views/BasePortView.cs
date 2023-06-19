using UnityEditor.Experimental.GraphView;


namespace XGraph 
{
	public class BasePortView : Port
	{
		protected BasePortView(): base(Orientation.Horizontal, Direction.Input, Capacity.Single, typeof(BaseNodeView))
		{
		}
	}
}