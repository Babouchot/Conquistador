package ihm.it.custom;

import org.achartengine.ChartFactory;
import org.achartengine.GraphicalView;
import org.achartengine.chart.AbstractChart;
import org.achartengine.model.CategorySeries;
import org.achartengine.renderer.DefaultRenderer;
import org.achartengine.renderer.SimpleSeriesRenderer;

import android.content.Context;
import android.graphics.Color;


public class PieChartView extends GraphicalView
{
 
	public static final int COLOR_GREEN = Color.parseColor("#62c51a");
	public static final int COLOR_ORANGE = Color.parseColor("#ff6c0a");
	public static final int COLOR_BLUE = Color.parseColor("#23bae9");
	public static final int COLOR_YELLOW = Color.parseColor("#f1c40f");
 
	/**
	 * Constructor that only calls the super method. It is not used to instantiate the object from outside of this
	 * class.
	 * 
	 * @param context
	 * @param arg1
	 */
	private PieChartView(Context context, AbstractChart arg1)
	{
		super(context, arg1);
	}
 
	/**
	 * This method returns a new graphical view as a pie chart view. It uses the income and costs and the static color
	 * constants of the class to create the chart.
	 * 
	 * @param context
	 *            the context
	 * @param income
	 *            the total income
	 * @param costs
	 *            the total cost
	 * @return a GraphicalView object as a pie chart
	 */
	public static GraphicalView getNewInstance(Context context, int income, int costs)
	{
		return ChartFactory.getPieChartView(context, getDataSet(context, income, costs), getRenderer());
	}
 
	/**
	 * Creates the renderer for the pie chart and sets the basic color scheme and hides labels and legend.
	 * 
	 * @return a renderer for the pie chart
	 */
	private static DefaultRenderer getRenderer()
	{
		int[] colors = new int[] { COLOR_GREEN, COLOR_ORANGE, COLOR_BLUE, COLOR_YELLOW };
 
		DefaultRenderer defaultRenderer = new DefaultRenderer();
		for (int color : colors)
		{
			SimpleSeriesRenderer simpleRenderer = new SimpleSeriesRenderer();
			simpleRenderer.setColor(color);
			defaultRenderer.addSeriesRenderer(simpleRenderer);
		}
		defaultRenderer.setShowLabels(false);
		defaultRenderer.setShowLegend(false);
		return defaultRenderer;
	}
 
	/**
	 * Creates the data set used by the piechart by adding the string represantation and it's integer value to a
	 * CategorySeries object. Note that the string representations are hard coded.
	 * 
	 * @param context
	 *            the context
	 * @param income
	 *            the total income
	 * @param costs
	 *            the total costs
	 * @return a CategorySeries instance with the data supplied
	 */
	private static CategorySeries getDataSet(Context context, int income, int costs)
	{
		CategorySeries series = new CategorySeries("Chart");
		return series;
	}
}