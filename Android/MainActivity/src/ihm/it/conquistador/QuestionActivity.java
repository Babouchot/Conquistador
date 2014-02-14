package ihm.it.conquistador;

import java.util.ArrayList;
import java.util.Collections;
import java.util.List;

import org.json.JSONException;
import org.json.JSONObject;

import android.app.Activity;
import android.content.Intent;
import android.graphics.Color;
import android.os.Bundle;
import android.os.SystemClock;
import android.os.Vibrator;
import android.text.Editable;
import android.util.Log;
import android.view.Menu;
import android.view.View;
import android.view.View.OnClickListener;
import android.widget.Button;
import android.widget.Chronometer;
import android.widget.EditText;
import android.widget.TextView;
import android.widget.Toast;

public class QuestionActivity extends Activity {

	private Chronometer chrono;
	private String id ;

	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);

		Intent intent = getIntent();
		JSONObject obj = null;
		try {
			/**
			 * OPEN 
			 */
			obj = new JSONObject(intent.getStringExtra("questionJSON"));
			Log.d("test",obj.toString());
			id = obj.getString("id").toString();
			if(obj.getString("type").equals("open")){
				setContentView(R.layout.activity_open_question);
				EditText edit =(EditText) findViewById(R.id.answerNumber);
				if( GameActivity.id.equals("0")){
					edit.setBackgroundColor(Color.RED);
				} else if( GameActivity.id.equals("1")){
					edit.setBackgroundColor(Color.YELLOW);
					edit.setTextColor(Color.BLACK);
				} else if(GameActivity.id.equals("2")){
					edit.setBackgroundColor(Color.GREEN);
					edit.setTextColor(Color.BLACK);
				} else {
					edit.setBackgroundColor(Color.BLUE);
				} 

				edit.setOnClickListener(new OnClickListener() {
					@Override
					public void onClick(View v) {
//						v.requestFocus();
					}
				});
			}
			else {//QCM
				setContentView(R.layout.activity_question);		
				List<String> answers=new ArrayList<String>();
				answers.add(obj.getString("answer"));
				answers.add(obj.getString("false1"));
				answers.add(obj.getString("false2"));
				answers.add(obj.getString("false3"));
				Collections.shuffle(answers);
				Button but = (Button) findViewById(R.id.answer1);
				but.setText(answers.get(0));
				but = (Button) findViewById(R.id.answer2);
				but.setText(answers.get(1));
				but = (Button) findViewById(R.id.answer3);
				but.setText(answers.get(2));
				but = (Button) findViewById(R.id.answer4);
				but.setText(answers.get(3));
			}
			TextView tvTitre = (TextView) findViewById(R.id.titreQuestion);
			tvTitre.setText(""+obj.getString("title"));
			
			if(GameActivity.id.equals("1") || GameActivity.id.equals("2")){
				tvTitre.setTextColor(Color.BLACK);
			}

		} catch (JSONException e) {
			Toast.makeText(getApplicationContext(), "JSON exception !", Toast.LENGTH_SHORT).show();
			finish();
		}
		//Changement du background
		View layout = findViewById(R.id.mainLayout);
		View topLayout = findViewById(R.id.topLayout);
		View questionLayout = findViewById(R.id.questionLayout);

		if(GameActivity.id.equals("0")){
			layout.setBackgroundResource(R.drawable.conqui1);
			topLayout.setBackgroundColor(Color.RED);
			questionLayout.setBackgroundColor(Color.RED);
		} else if(GameActivity.id.equals("1")){
			layout.setBackgroundResource(R.drawable.conqui2);
			topLayout.setBackgroundColor(Color.YELLOW);
			questionLayout.setBackgroundColor(Color.YELLOW);
		} else if(GameActivity.id.equals("2")){
			layout.setBackgroundResource(R.drawable.conqui3);
			topLayout.setBackgroundColor(Color.GREEN);
			questionLayout.setBackgroundColor(Color.GREEN);
		} else {
			layout.setBackgroundResource(R.drawable.conqui4);
			topLayout.setBackgroundColor(Color.BLUE);
			questionLayout.setBackgroundColor(Color.BLUE);
		} 
		setContentView(layout);

		TextView tvp = (TextView) findViewById(R.id.pseudo);
		TextView title = (TextView) findViewById(R.id.title);
		tvp.setText(ConnectionSocket.pseudo);
		//Lancement du chronometre et du timer
		chrono = (Chronometer) findViewById(R.id.chronometer);
		chrono.setBase(SystemClock.elapsedRealtime());
		chrono.start();

		if(GameActivity.id.equals("1") || GameActivity.id.equals("2")){
			tvp.setTextColor(Color.BLACK);
			title.setTextColor(Color.BLACK);
			chrono.setTextColor(Color.BLACK);
		}
		Vibrator vibrator = (Vibrator) getSystemService(Activity.VIBRATOR_SERVICE);		
		vibrator.vibrate(200);	
	}

	public void SendClick(View v){
		chrono = (Chronometer) findViewById(R.id.chronometer);
		chrono.stop();
		long elapsedMillis = SystemClock.elapsedRealtime() - chrono.getBase();
		//Generation de l'intent
		Intent returnIntent = new Intent();
		//Gestion des extras
		returnIntent.putExtra("answer",((Button)v).getText().toString());
		returnIntent.putExtra("time", elapsedMillis+"" );
		returnIntent.putExtra("id", id);
		setResult(GameActivity.RESULT_ANSWER,returnIntent);   
		finish();
	}

	public void sendNumber(View v) {
		//Gestion du chrono
		chrono = (Chronometer) findViewById(R.id.chronometer);
		chrono.stop();
		long elapsedMillis = SystemClock.elapsedRealtime() - chrono.getBase();
		float elapsedSeconds = elapsedMillis /1000;
		//Generation de l'intent
		Intent returnIntent = new Intent();
		EditText edit =(EditText)  findViewById(R.id.answerNumber);
		//Gestion des extras
		returnIntent.putExtra("answer",edit.getText().toString());
		returnIntent.putExtra("time", elapsedSeconds+"" );
		returnIntent.putExtra("id", id);
		setResult(GameActivity.RESULT_ANSWER,returnIntent);   
		finish();
	}

	public void cancelClick(View v){
		EditText edit =(EditText)  findViewById(R.id.answerNumber);
		edit.setText("");
	}
	
	public void updateChiffre(View v){

		EditText edit =(EditText)  findViewById(R.id.answerNumber);
		Editable t = edit.getText();
		t.append(((Button) v).getText().toString());	
		edit.setText(t);
	}

	@Override
	public boolean onCreateOptionsMenu(Menu menu) {
		// Inflate the menu; this adds items to the action bar if it is present.
		getMenuInflater().inflate(R.menu.question, menu);
		return true;
	}

}
