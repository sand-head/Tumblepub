# Creating a custom theme

Theme creation is designed to as closely resemble Tumblr theme creation as possible using [Handlebars](https://handlebarsjs.com/). Unlike Tumblr, however, no additional scripts or elements are inserted into a theme at runtime.

The following is an example of a complete theme:
```hbs
<!DOCTYPE html>
<html>
  <head>
    <title>{{Title}}</title>
    {{#if Description}}
    <meta name="description" content="{{Description}}" />
    {{/if}}
  </head>
  <body>
    {{Content}}
  </body>
</html>
```

## Basic variables

Variable | Description
--- | ---
`{{Title}}` | Your blog's title.
`{{Description}}` | Your blog's description

## Temporary variables

Variable | Description
--- | ---
`{{Content}}` | Your blog's content.

## Custom Handlebars helpers

Helper | Description
--- | ---
`{{url ...}}` | URL-encodes a given variable.