# Serial protocol for Arduino Control

`@Appp:nnn;` - Analog write pin ppp value nnn, where nnn is between 0 and 255, inclusive.
               Arduino will respond with `@OK;` or `@ERROR...;` depending if this is a valid write.

`@Dppp:n;` - Digital write pin ppp value n, where n is 0 or 1.
             Arduino will respond with `@OK;` or `@ERROR...;` depending if this is a valid write.

`@Ippp;` - Read from input pin ppp.  Arduino will respond with one of the above two commands.

`@RESET;` - Reset pins to default startup values.

`@LIST;` - Request a list of available pins and their assigned names, values, and types.  See below.

`@POLL;` - A shorter version of `@LIST;` that returns only input pins, minus the names.

## List response

The response is of the format:  `@LIST` <any number of pin descriptors> `;`

A pin descriptor is as follows:

* First character is the vertical pipe char, `|`
* Second character is `A` or `D`, for Analog or Digital.
* Third character is `I` or `O`, for Input (read from hardware) or Output (write to hardware).
* One or more digit characters indicate the pin number in decimal.
* A colon ':' followed by the current value of the pin.
* A double-quote character indicates the start of the pin's human-readable name.
* The backslash is used to escape backslashes, @'s, and double-quotes within the pin name.
* The next un-escaped double-quote marks the end of the pin name and descriptor.
* Any un-escaped @ sign means the list terminated prematurely.

## Example list

Let's say we have a board with three analog outs on pins 3, 5, and 6, and two analog input
pins (one grounded and one high).  The Arduino's response to the `@LIST;` command might
look like this:

```
@LIST|AO3:255"Red LED"|AO5:255"Green LED"|AO6:0"Blue LED"|AI18:0"Input 1"|AI19:1023"Input 2";
```

## Poll response

This is similar to the `@LIST;` response, but returns only pins set to "input" mode, and does
not include names or the I/O indicator.  It is meant for continual polling of the assigned
input pins, at a rate established by the host PC.

Here's what `@POLL;` would respond to the same situation that produced the list example above:

```
@POLL|A18:0|A19:1023;
```

## Push updates

Digital input pins will push their updates without polling, using `@PUSH`.

```
@PUSH|D12:0;
```

## Additional responses

`@OK;` - Write operation succeded

`@ERROR: <message>;` - Operation failed.
