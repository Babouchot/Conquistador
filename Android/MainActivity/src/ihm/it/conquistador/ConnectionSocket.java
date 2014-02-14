package ihm.it.conquistador;

import io.socket.IOAcknowledge;
import io.socket.IOCallback;
import io.socket.SocketIO;
import io.socket.SocketIOException;

import org.json.JSONException;
import org.json.JSONObject;

import android.content.Context;
import android.content.Intent;

public class ConnectionSocket {

	private static SocketIO socket = null;
	public static String url;
	public static String pseudo="";
	public static Context ctx;
	private static ConnectionSocket INSTANCE = null;

	private ConnectionSocket(){

	}

	public void Connection(){
		try {
			socket = new SocketIO(url);
			IOCallback call = new MyCallBack(socket,pseudo);
			socket.connect(call);
		} catch (Exception e) {
			e.printStackTrace();
		}	 
	}

	public static void Emit(String name, Object... args){
		socket.emit(name,args);
	}


	public static ConnectionSocket getInstance(){
		if(INSTANCE==null) INSTANCE = new ConnectionSocket();
		return INSTANCE;
	}

	public class MyCallBack implements IOCallback{

		private SocketIO socket;
		private String _name;

		public MyCallBack(SocketIO socket,String name){
			this.socket = socket;
			_name = name;
		}

		@Override
		public void onMessage(JSONObject json, IOAcknowledge ack) {
			try {
				System.out.println("Server said:" + json.toString(2));
			} catch (JSONException e) {
				e.printStackTrace();
			}
		}

		@Override
		public void onMessage(String data, IOAcknowledge ack) {
			System.out.println("Server said: " + data);
		}

		@Override
		public void onError(SocketIOException socketIOException) {
			System.out.println("an Error occured");
			socketIOException.printStackTrace();
		}

		@Override
		public void onDisconnect() {
			System.out.println("Connection terminated.");
		}

		@Override
		public void onConnect() {
			System.out.println("Connected.");
		}

		@Override
		public void on(String event, IOAcknowledge ack, Object... args) {

			System.out.println("Server triggered event '" + event + "'");

			if(event.equalsIgnoreCase("requestIdentity")){				 
				JSONObject json = null;
				try {
					json = new JSONObject("{\"pseudo\":\""+_name+"\"}");
				} catch (JSONException e) {
					e.printStackTrace();
				}
				socket.emit("playerIdentity",json);

			} else if(event.equalsIgnoreCase("question")){
				Intent question = new Intent(ctx,QuestionActivity.class);
				JSONObject jspn = (JSONObject) args[0];
				question.putExtra("questionJSON", jspn.toString());
				((GameActivity) ctx).startActivityForResult(question, 0);
//				GameActivity.id="3";
//				ctx.startActivity(question);

			}
			else if(event.equalsIgnoreCase("successfullyConnected")){
				Intent wait = new Intent(ctx,WaitActivity.class);
				ctx.startActivity(wait);
//				socket.emit("requestQuestionTest");
			}

			else if(event.equalsIgnoreCase("startGame")){
				JSONObject jspn = (JSONObject) args[0];
				try {
					Intent game = new Intent(ctx,GameActivity.class);
					game.putExtra("gamerId", jspn.get("id").toString());
					ctx.startActivity(game);
				} catch (JSONException e) {
					e.printStackTrace();
				}
			}
			else if(event.equalsIgnoreCase("majChart")){ 
				((GameActivity) ctx).Refresh(args);
			}
		}
	}


}
