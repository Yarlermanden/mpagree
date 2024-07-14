mod pb {
	tonic::include_proto!("mod");
}

pub mod validator {
	pub use crate::pb::validator::*;
}

pub mod grpc;

pub mod model;
