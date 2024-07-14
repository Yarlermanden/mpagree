use engine::{
	self,
	validator::{ConnectPlayerRequest, UpdatePlayerRequest},
};

use crate::engine::validator::validator_client::ValidatorClient;

#[tokio::main]
async fn main() -> Result<(), Box<dyn std::error::Error>> {
	println!("Hello, world!1");
	println!("validator test client is starting up...");

	let _env: &str = "local";
	let port: i32 = 5003;

	let mut client = ValidatorClient::connect(format!("http://0.0.0.0:{}", port)).await?;

	let connect_req = tonic::Request::new(ConnectPlayerRequest {
		ip_address: "localhost".to_string(),
		username: "player1".to_string(),
	});

	let connect_resp = client.connect_player(connect_req).await?.into_inner();

	for _ in 0..10 {
		let request = tonic::Request::new(UpdatePlayerRequest {
			player_id: connect_resp.player_id,
			delta_x: -1.0,
			delta_y: 0.0,
		});

		let response = client.update_player(request).await?;
		let res = response.into_inner();
		println!("RESPONSE: {:?}", res);
	}

	let current_state = client.get_state(()).await?.into_inner();
	println!("{}", "Current state: ");
	for p in current_state.players {
		println!("Player id: {}, x: {}, y: {}", p.id, p.x, p.y);
	}

	Ok(())
}
