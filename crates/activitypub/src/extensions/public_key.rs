use activitystreams::unparsed::UnparsedMutExt;
use activitystreams_ext::UnparsedExtension;
use serde::{Deserialize, Serialize};

use crate::models;

#[derive(Deserialize, Serialize)]
#[serde(rename_all = "camelCase")]
pub struct PublicKey {
  pub public_key: models::actor::PublicKey,
}

impl<U> UnparsedExtension<U> for PublicKey
where
  U: UnparsedMutExt,
{
  type Error = serde_json::Error;

  fn try_from_unparsed(unparsed_mut: &mut U) -> Result<Self, Self::Error>
  where
    Self: Sized,
  {
    Ok(PublicKey {
      public_key: unparsed_mut.remove("public_key")?,
    })
  }

  fn try_into_unparsed(self, unparsed_mut: &mut U) -> Result<(), Self::Error> {
    unparsed_mut.insert("public_key", self.public_key)?;
    Ok(())
  }
}
