use jsonwebtoken::{decode, encode, DecodingKey, EncodingKey, Header, Validation};
use serde::{Deserialize, Serialize};

#[derive(Deserialize, Serialize)]
pub struct UserClaims {
  pub sub: i64,
  // todo: add user claims
}

pub struct Token {
  claims: Option<UserClaims>,
}
impl Token {
  pub fn new(claims: UserClaims) -> Self {
    Token {
      claims: Some(claims),
    }
  }

  pub fn from(token: String) -> Self {
    let validation = Validation {
      validate_exp: false,
      ..Validation::default()
    };

    let claims = decode::<UserClaims>(
      &token,
      // todo: do not hardcode the secret
      &DecodingKey::from_secret("big boy secret time".as_bytes()),
      &validation,
    )
    .ok()
    .map(|data| data.claims);

    Token { claims }
  }

  pub fn get_claims(&self) -> &Option<UserClaims> {
    &self.claims
  }

  pub fn generate(&self) -> Result<String, anyhow::Error> {
    match &self.claims {
      None => Err(anyhow::anyhow!(
        "could not generate JWT with no user claims"
      )),
      Some(claims) => Ok(
        encode(
          &Header::default(),
          claims,
          // todo: do not hardcode the secret
          &EncodingKey::from_secret("big boy secret time".as_bytes()),
        )
        .unwrap(),
      ),
    }
  }
}
