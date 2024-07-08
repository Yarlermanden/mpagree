use crate::{
	engine::{
		engine_server::Engine, ConnectPlayerRequest, ConnectPlayerResponse, GetStateResponse, UpdatePlayerRequest, UpdatePlayerResponse,
	},
	model::state::World,
};
use std::sync::Mutex;

use tonic::{Request, Response, Status};

pub struct Server {
	world: Mutex<World>,
}

impl Server {
	pub fn new() -> Self {
		Server {
			world: Mutex::new(World::new()),
		}
	}
}

#[tonic::async_trait]
impl Engine for Server {
	async fn connect_player(&self, request: Request<ConnectPlayerRequest>) -> Result<Response<ConnectPlayerResponse>, Status> {
		let req = request.into_inner();
		let player_id = self.world.lock().unwrap().add_player(req.username);
		let resp = ConnectPlayerResponse {
			player_id,
		};
		Ok(Response::new(resp))
	}

	async fn update_player(&self, request: Request<UpdatePlayerRequest>) -> Result<Response<UpdatePlayerResponse>, Status> {
		let req = request.into_inner();

		let player_id = self.world.lock().unwrap().update_player(req.player_id, req.delta_x, req.delta_y);
		match player_id {
			Some(_) => Ok(Response::new(UpdatePlayerResponse {
				success: true,
			})),
			None => Err(Status::not_found("couldn't find player with id")),
		}
	}

	async fn get_state(&self, _request: Request<()>) -> Result<Response<GetStateResponse>, Status> {
		let players = self.world.lock().unwrap().get_players().into_iter().map(|x| x.clone()).collect();

		let resp = GetStateResponse {
			players,
		};

		Ok(Response::new(resp))
	}
}
