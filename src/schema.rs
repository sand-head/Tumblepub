table! {
    blogs (id) {
        id -> Int8,
        uri -> Nullable<Varchar>,
        name -> Varchar,
        domain -> Nullable<Varchar>,
        title -> Nullable<Varchar>,
        description -> Nullable<Text>,
    }
}

table! {
    user_blogs (user_id, blog_id) {
        user_id -> Int8,
        blog_id -> Int8,
        is_admin -> Nullable<Bool>,
    }
}

table! {
    users (id) {
        id -> Int8,
        email -> Varchar,
        encrypted_password -> Varchar,
        primary_blog -> Int8,
    }
}

joinable!(user_blogs -> blogs (blog_id));
joinable!(user_blogs -> users (user_id));
joinable!(users -> blogs (primary_blog));

allow_tables_to_appear_in_same_query!(
    blogs,
    user_blogs,
    users,
);
