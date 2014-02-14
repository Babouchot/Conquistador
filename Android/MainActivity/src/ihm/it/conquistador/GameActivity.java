package ihm.it.conquistador;

import io.socket.SocketIO;

import org.achartengine.ChartFactory;
import org.achartengine.GraphicalView;
import org.achartengine.model.CategorySeries;
import org.achartengine.renderer.DefaultRenderer;
import org.achartengine.renderer.SimpleSeriesRenderer;
import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;

import android.app.Activity;
import android.content.Intent;
import android.graphics.Color;
import android.os.Bundle;
import android.util.Log;
import android.view.Menu;
import android.view.View;
import android.widget.LinearLayout;
import android.widget.TextView;

public class GameActivity extends Activity {

	public static final int RESULT_ANSWER=1;
	public static String id;

	SocketIO socket = null;
	private static final CategorySeries mSeries = new CategorySeries("");
	private DefaultRenderer mRenderer = new DefaultRenderer();
	public static GraphicalView mChartView;

	private GraphicalView mChart=null;

	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);

		Intent intent = getIntent();
		id = intent.getStringExtra("gamerId");

		setContentView(R.layout.activity_game);

		View layout = findViewById(R.id.mainLayout);
		if(id.equals("0")){
			layout.setBackgroundResource(R.drawable.conqui1);
			MakeChart(ConnectionSocket.pseudo,"","","");
		} else if(id.equals("1")){
			layout.setBackgroundResource(R.drawable.conqui2);
			MakeChart("",ConnectionSocket.pseudo,"","");
		} else if(id.equals("2")){
			layout.setBackgroundResource(R.drawable.conqui3);
			MakeChart("","",ConnectionSocket.pseudo,"");
		} else {
			layout.setBackgroundResource(R.drawable.conqui4);
			MakeChart("","","",ConnectionSocket.pseudo);
		} 
		setContentView(layout);

		ConnectionSocket.ctx = this;
	}


	public void Refresh(Object... args){
		if(mChart == null) {return;}
		mSeries.clear();
		JSONArray jarray = (JSONArray) args[0];
		try {
			Log.d("json","length "+jarray.length());
			for(int i = 0;i<jarray.length();i++){
				JSONObject json = (JSONObject) jarray.get(i);
				Log.d("json",json.toString());
				final String pseudo = json.getString("pseudo");
				final String score = json.getString("score");
				Double sc = Double.valueOf(score);
				mSeries.add(pseudo,sc);
				if(i==0){
					final TextView tv = (TextView) findViewById(R.id.player1);
					final TextView tvs = (TextView) findViewById(R.id.score1);
					this.runOnUiThread(new Runnable() {

						@Override
						public void run() {
							// TODO Auto-generated method stub

							tv.setText(pseudo);
							tvs.setText(score);
						}
					});
				} else if(i==1) {
					final TextView tv = (TextView) findViewById(R.id.player2);
					final TextView tvs = (TextView) findViewById(R.id.score2);
					this.runOnUiThread(new Runnable() {

						@Override
						public void run() {
							// TODO Auto-generated method stub

							tv.setText(pseudo);
							tvs.setText(score);
						}
					});
				}else if(i==2) {
					final TextView tv = (TextView) findViewById(R.id.player3);
					final TextView tvs = (TextView) findViewById(R.id.score3);
					this.runOnUiThread(new Runnable() {

						@Override
						public void run() {
							// TODO Auto-generated method stub

							tv.setText(pseudo);
							tvs.setText(score);
						}
					});
				}else if(i==3) {
					final TextView tv = (TextView) findViewById(R.id.player4);
					final TextView tvs = (TextView) findViewById(R.id.score4);
					this.runOnUiThread(new Runnable() {

						@Override
						public void run() {
							// TODO Auto-generated method stub

							tv.setText(pseudo);
							tvs.setText(score);
						}
					});
				}
			}
		} catch (Exception e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}

		mChart.repaint();
		
	}

	@Override
	public boolean onCreateOptionsMenu(Menu menu) {
		return true;
	}

	@Override
	protected void onActivityResult(int requestCode, int resultCode, Intent data) {
		if(resultCode != RESULT_CANCELED){
			super.onActivityResult(requestCode, resultCode, data);
			if(resultCode == RESULT_ANSWER){
				String answer=data.getStringExtra("answer");   
				String time=data.getStringExtra("time");    
				String id=data.getStringExtra("id");   

				JSONObject json = null;
				try {
					json = new JSONObject("{\"answer\":\""+answer+"\",\"time\":\""+time+"\",\"id\":\""+id+"\"}");
				} catch (JSONException e) {
					e.printStackTrace();
				}
				ConnectionSocket.Emit("answer",json);
			}
		}
	}


	private void MakeChart(String name1,String name2,String name3, String name4){      
		mSeries.add(name1, 25);
		mSeries.add(name2, 25);
		mSeries.add(name3, 25);
		mSeries.add(name4, 25);
		//CHART
		mRenderer.setApplyBackgroundColor(true);
		mRenderer.setChartTitleTextSize(20);
		mRenderer.setLabelsTextSize(15);
		mRenderer.setLegendTextSize(15);
		mRenderer.setStartAngle(25);
		SimpleSeriesRenderer renderer = new SimpleSeriesRenderer();
		renderer.setColor(Color.RED);
		mRenderer.addSeriesRenderer(renderer);
		renderer = new SimpleSeriesRenderer();
		renderer.setColor(Color.YELLOW); 
		mRenderer.addSeriesRenderer(renderer);
		renderer = new SimpleSeriesRenderer();
		renderer.setColor(Color.GREEN); 
		mRenderer.addSeriesRenderer(renderer);
		renderer = new SimpleSeriesRenderer();
		renderer.setColor(Color.BLUE); 
		mRenderer.addSeriesRenderer(renderer);
		mChart = ChartFactory.getPieChartView(getBaseContext(), mSeries, mRenderer);
		LinearLayout chartContainer = (LinearLayout) findViewById(R.id.chart_container);
		chartContainer.addView(mChart); 
	}

}

