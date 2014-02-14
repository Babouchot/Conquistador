package ihm.it.conquistador;

import android.provider.MediaStore.Images;

public class Joueur {

	private String _pseudo;
	private Images _profilPicture;
	
	public Joueur(){
		_pseudo = "Guest";
	}
	
	public Joueur(String prenom, String nom){
		_pseudo = prenom;
	}
	
	public void setProfilPicture(Images pp){
		_profilPicture = pp;
	}
}
