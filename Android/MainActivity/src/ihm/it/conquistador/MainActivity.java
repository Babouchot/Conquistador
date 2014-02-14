package ihm.it.conquistador;

import ihm.it.custom.IntentIntegrator;
import ihm.it.custom.IntentResult;
import android.app.Activity;
import android.content.Intent;
import android.graphics.Typeface;
import android.os.Bundle;
import android.view.Menu;
import android.view.View;
import android.widget.EditText;
import android.widget.TextView;
import android.widget.Toast;

public class MainActivity extends Activity {

	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		
		setContentView(R.layout.activity_main);

		TextView title = (TextView) findViewById(R.id.title);
		TextView slogan = (TextView) findViewById(R.id.slogan);
		setFont(title,"fonts/Ruritania.ttf");
		setFont(slogan,"fonts/Ruritania.ttf");
		
		EditText login = (EditText) findViewById(R.id.pseudo);
		setFont(login,"fonts/dali.ttf");
	}

	public void ScanClick(View v) {
		IntentIntegrator integrator = new IntentIntegrator(this);
		integrator.initiateScan();
	}

	public void onActivityResult(int requestCode, int resultCode, Intent intent) {

		if (resultCode == RESULT_CANCELED) {
			return;
		}

		IntentResult scanResult = IntentIntegrator.parseActivityResult(requestCode, resultCode, intent);
		if (scanResult != null) {
			String contents = scanResult.getContents();				
			Intent game = new Intent(this,GameActivity.class);
			game.putExtra("IP", contents);
			EditText pseudo = (EditText) findViewById(R.id.pseudo);

			if(IsValidUrl(contents)){
				Toast.makeText(this, "Valid URL", Toast.LENGTH_SHORT).show();
				ConnectionSocket.url = contents;
				ConnectionSocket.pseudo = pseudo.getText().toString();
				ConnectionSocket.ctx=this;
				ConnectionSocket.getInstance();
				ConnectionSocket.getInstance().Connection();
			}else{
				Toast.makeText(this, "Bad URL", Toast.LENGTH_SHORT).show();
			}
		}
	}
	
	private boolean IsValidUrl(String url){
		return (url.startsWith("http://") && (url.endsWith(":8080") || url.endsWith(":8080/")) && url.split(":").length==3);
	}
	
	public static void setFont(TextView textView,String font) {
	    Typeface tf = Typeface.createFromAsset(textView.getContext().getAssets(), font);
	    textView.setTypeface(tf);
	}
	
	@Override
	public boolean onCreateOptionsMenu(Menu menu) {
		// Inflate the menu; this adds items to the action bar if it is present.
		getMenuInflater().inflate(R.menu.main, menu);
		return true;
	}

}
