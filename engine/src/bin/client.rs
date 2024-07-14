use std::time::SystemTime;

use engine::{
	self,
	server::{player_event::Event::*, ConnectPlayerRequest, MovePlayerEvent, PlayerEvent, QueueEventRequest},
};
use prost_types::Timestamp;

use crate::engine::server::queue_client::QueueClient;

#[tokio::main]
async fn main() -> Result<(), Box<dyn std::error::Error>> {
	println!("Hello, world!1");
	println!("client is starting up...");

	let _env: &str = "local";
	let port: i32 = 5002;

	let mut client = QueueClient::connect(format!("http://0.0.0.0:{}", port)).await?;

	let connect_req = tonic::Request::new(ConnectPlayerRequest {
		ip_address: "localhost".to_string(),
		username: "player1".to_string(),
	});

	let connect_resp = client.connect_player(connect_req).await?.into_inner();

	for _ in 0..10 {
		let request = tonic::Request::new(QueueEventRequest {
			event: Some(PlayerEvent {
				player_id: connect_resp.player_id,
				requested_time: Some(Timestamp::from(SystemTime::now())),
				event: Some(MovePlayer(MovePlayerEvent {
					delta_x: -1.0,
					delta_y: 0.0,
				})),
			}),
		});

		let response = client.queue_event(request).await?;
		let res = response.into_inner();
		println!("RESPONSE: {:?}", res);
	}

	let events = client.get_and_clear_events(()).await?.into_inner();
	println!("{}", "Current events: ");
	for e in events.events {
		println!("player id: {}, requested_time: {}, event: {:?}", e.player_id, e.requested_time.unwrap(), e.event.unwrap());
	}

	Ok(())
}
