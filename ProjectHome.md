Network Working Group							J. Sund
Internet Draft								PSU
Intended status: educational                				June 3, 2010

> Internet Chat Reliable Application Protocol

Abstract

> Internet Chat Reliable Application Protocol is simplified version of the
> Internet Relay Chat Protocol (RFC 1459), which will be referred to
> throughout this document as ICRAP. ICRAP allows for multiple clients to
> hold TCP connections to a single server for the purpose of posting and
> receiving plain text messages among a network of connected clients.
> Following a simple client-server model, the client maintains a TCP
> socket connection to the server on a know port which connects to an
> ICRAP controller.

Status of this Memo

> This document is a preliminary view of the ICRAP solution. Stated in
> this document are the un-tested and un-tried ideas for the Internet Chat
> Reliable Application Protocol. While the information in this document at
> its initial time of writing is complete and well described, application
> development to test the methods laid out as such has not yet been
> completed. The reader should expect changes in this document as the
> ICRAP solution continues to be developed.

> Version Control:

> Version		Comments			By				Released On
  1. initial release		Jared Sund		4/30/2011
> 2			PreRelease			Jared Sund		6/2/2011

Copyright Notice

> Copyright (c) 2011 Jared T. Sund, All rights reserved.

> This document is protected by the MIT License as listed below:

> Permission is hereby granted, free of charge, to any person obtaining a
> copy of this software and associated documentation files (the
> "Software"), to deal in the Software without restriction, including
> without limitation the rights to use, copy, modify, merge, publish,
> distribute, sublicense, and/or sell copies of the Software, and to
> permit persons to whom the Software is furnished to do so, subject to
> the following conditions:

> The above copyright notice and this permission notice shall be included
> in all copies or substantial portions of the Software.

> THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
> OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
> MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.

Sund			                                                	[1](Page.md)

RFC xxxx  		Internet Chat Reliable Application Protocol       	May 2011

> IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY
> CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
> TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE
> SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

Table of Contents

  1. Introduction .................................................    1
    1. 1   .................................................    1
  1. Introduction .................................................    1
  1. Introduction .................................................    1
  1. Introduction .................................................    1
  1. Introduction .................................................    1
  1. Introduction .................................................    1
  1. Introduction .................................................    1
  1. Introduction .................................................    1
  1. Introduction .................................................    1
  1. Introduction .................................................    1
  1. Introduction .................................................    1
  1. Introduction .................................................    1


1. Introduction

> The Internet Chat Reliable Application Protocol (ICRAP) is a text based
> messaging system, used to distribute/multiplex messages from a single
> client to many recipient clients. Clients both post and receive messages
> to/from many channels, which are centralized on a single server. Clients
> will connect directly to a given ICRAP server, which will maintain both
> a controlling agent as well as individual channels for the purpose of
> receiving and re-broadcasting messages to 1 or more connected clients.

> The initial implementation of the ICRAP system will be based on TCP/IP
> socket connections to maintain reliable message transfer. The protocol
> could however be revised to implement different transport protocols such
> as UDP.

> In this document the reader will find all the details required to
> implement both server and client applications needed to implement an
> ICRAP system.

1.1 Server

> The server is responsible for controlling much of the text based
> messaging activity, as well as the creation and listing of the channels.
> Communication will be managed via TCP socket connections for both
> server-to-self and server-to-client messaging. While many individual
> servers can be erected, each server is a standalone ICRAP solution. No
> server to server communications will be supported in this protocol.

> The server has two base modules: the controller, and the channel(s).
> These two modules provide the basis for all text messaging activities
> between clients.

1.1.1 Controller

> The controller is the main interface for a client to interact with the
> ICRAP server. The controller will accept connections to a specified port
> which is defined on initialization of the server process. This listening
> port will be established and documented for client connections to the
> server. It should be recognized that this port should be maintained for
> the life of the server, so that clients will be able to make connections
> to the server at anytime. If the controller is allowed to auto generate
> its port on initialization, documentation such as a web page should be
> made available for clients to know which port to connect to.

> The controller’s functions include initial client authentication,
> channel creation, destruction, and enumeration. The controller then
> provides the clients with high level administrative functions for
> operating with a give ICRAP server.

> Client authentication will only be granted for a client that provides a
> valid port to respond to, and a client user’s nickname. The controller
> will not maintain a list of clients during its life, and therefore will
> be considered stateless for client connections. While a persistent TCP
> connection is available, the client can send and receive message across
> that connection. If the connection is closed, the client can simply open
> a new connection and resume activity due to the stateless nature of the
> controller.

> Channel creation and destruction will be the sole responsibility of the
> controller. Upon creation of a new channel, the controller will generate
> a port number to be allocated for the new channel as a listening port.
> This port number must be unique on the server, and will serve as the
> entrance to the channel. The server will also maintain uniqueness for
> the channel’s textual description/name. If a channel creation is
> requested by a client, the controller will be responsible for
> construction of this channel that is if the descriptive name uniqueness
> can be maintained. A status message will be provided to the client with
> the success or error for a given clients request.

> The controller will also provide enumeration of existing channel(s) to a
> requesting client. This will include both the port number for
> connection, in addition to the descriptive name for each available
> channel.

> Administrative functions will be granted to clients for tasks such as
> destruction of channels, adjustment of variables, and disabling of a
> particular client. The ICRAP server does not maintain a list of known
> clients (username and password), so the client will be defined as an
> administrator if it is running on the same host as the server (i.e.
> localhost). For certain administrative functions (such as killing a
> client), the controller will need to verify the incoming requestor’s IP
> address against its own before allowing this action to take place. If an
> administrator sets the maximum number of channels to a value that is
> less than the existing set of channels, those channels will operate as
> normal and no new channels will be created. Once the number of channels
> falls below the maximum number of channels allowed, new channels can
> again be created.

> Additionally, the controller will communicate with channels via TCP
> connections. This provides the same administrative functionality of
> connections to channels via same host connections.

> Controller commands:

> ENUMCHAN		- provides a list of channels (names and ports)

> CREATECHAN 		– allows for the creation of a new channel

> DESTCHAN 		– provides administrative functionality to
> > dispose of a channel


> SETMAXCHAN 		– provides an administrator with the ability to
> > set the maximum number of channels that can be
> > created on a server.


> SETMAXCLIENTS	– provides an administrator with the ability to
> > set the maximum number of clients that can
> > connect to a channel.


> SYSMESSAGE 		– provides an administrator the ability to post
> > a messages to all available channels


> VERSION 		– provides the server version, so that the client
> > will know which version of this document is
> > supported.

1.1.2 Channel


> The channel is the main text based messaging portion of the ICRAP
> solution. Channels receive posted messages from one or more clients, and
> re-distribute these messages to all clients connected to the channel
> (including the poster).

> Connection to a channel is provided through an open welcome port
> (established by the controller), where the channel is listening for TCP
> connections. The port and channel description is provided by the
> controller to the client. When a client requests connection to the
> channel, the channel with either accept the connection or in the case
> that the channel has reached a maximum number of allowed connections it
> will refuse the requesting client access. Upon acceptance to the
> channel, the channel will create and maintain a new TCP connection
> between the requesting client and itself. Each channel will maintain
> individual lists of connections to it. Channels are not aware of other
> channels on the server, nor their client connections.

> Once a channel has initialized a TCP connection for a given client, the
> client is able to send and receive messages to that channel across the
> connection. If the connection should be lost, the client will be
> required to request a new connection to the channel.

> Like the controller, the channel offers administrative functions. An
> administrator is determined by a client (normal client or the
> controller) that is running on the same host (localhost) as itself.
> Administrative functions include closing of the channel and setting or
> updating the maximum number of clients the channel can accept. If the
> administrator sets a maximum number of clients which is less than the
> number of clients already connected to the channel, the channel will
> maintain all the existing connections but will not accept new client
> connections until the number of client connections is below the maximum
> set variable.

> Each channel will maintain an activity countdown timer. On each
> transaction the channel will reset this time to maintain that the
> channel is still active. After a set amount of time (defined by the
> administrator at initialization of the server), the channel will request
> from the controller to have the channel closed if the timer counts to
> the value of zero from lack of activity. In this case the controller
> will close the channel.

> Channel Commands:

> ACCEPTCLIENT 	initializes a TCP connection between the client and
> > channel


> CLOSECONN 		allows the client to be removed from the channel

> ENUMCLIENTS 	provides a list of all the clients connected to the
> > channel


> POSTMESSAGE 	accepts an incoming message from a client

> KILLCLIENT 		administrative function to close connection for a given
> > client


> SETMAXCLIENTS 	provides an administrator with the ability to set the
> > maximum number of clients that can connect to a channel.


1.2 Client


> A client can be any device that is capable of establishing a TCP socket
> connection with an ICRAP server. The client is a relatively simple
> application that requests connection to the controller and then
> channels. Between the controller and channels, the client is capable of
> sending and receiving messages through established TCP connections. If a
> connection should be lost between the controller and channel(s), the
> client will be responsible for requesting a new connection. The server
> will not request TCP connections with the clients.

> Once a connection is established with channel, the client can post
> messages to that channel. Clients may establish multiple connections
> with different channels, however message posting will be on a channel by
> channel basis. When posting a message, the client will receive
> confirmation of the post from the channel. The channel will also provide
> the message back to the client as if it were a message from any number
> clients connected to this channel. This provides the sending client with
> the same sequencing of broadcasted message as all other clients
> connected to the channel.

> For an up to date list of available commands and messages, the
> application client programmer should refer to this document for
> instructions. The client can ask the server for its version, this will
> let the client know which commands and messages formats that are
> supported on a given ICRAP server.

> The client will be responsible for maintaining a list of socket
> connections to channels. If the client wishes to post a message to a
> channel, it will be the responsibility of the client to maintain which
> channel it is posting to.

> Client Commands:

> ACCEPTMESSAGE 	to receive messages from a connected to channel


2. Messages

> All messages between the server (controller or channels) and client will
> be in a streaming meta-tag language (XML) that is defined in this
> document. A message will consist of a single root element with a given
> number of sub elements in a hierarchical form. Root message elements
> will be either a request element or a response element. Each root
> element (request or response) will have a predefined set of sub elements
> depending on the command or success of the command.

2.1 Request

> A request can be thought of as a request for action. This could include
> a client requesting an enumeration of all channels from the server
> controller, or a client requesting to post a message to a particular
> channel. For any request, the basic format is as follows:

> 

&lt;xsd:element name="Request"&gt;


> > 

&lt;xsd:attribute name=”command” type=”xsd:string” /&gt;


> > 

&lt;xsd:attribute name=”clientname” type=”xsd:string” /&gt;


> > <xsd:attribute name=”data” type”xsd:string” />

> 

&lt;/xsd:element&gt;



2.2 Response

> A response message will be provided for all requests. The response
> message will include status about the success or failure of the request,
> readable comments, and could include the data result from the request.
> For any response, the basic format is as follows:

> 

&lt;xsd:element name="Response"&gt;


> > 

&lt;xsd:attribute name=”status” type=”xsd:string” /&gt;


> > 

&lt;xsd:attribute name=”comment” type=”xsd:string” /&gt;


> > <xsd:attribute name=”data” type”xsd:string” />

> 

&lt;/xsd:element&gt;



> The data attribute is optional, but could include nested elements
> depending on the command it is responding to.

> Status messages for a response will be either:

> SUCCESS
> ERROR

3. Message Syntax

3.1 Controller

3.1.1 EnumChan

> Example:  

&lt;Request command=”EnumChan” clientname=”foo” /&gt;



> Success Response: A successful response will include the “Success”
> status in the response message, empty comment, and a data nested
> hierarchy as follows:

> 

&lt;xsd:element name="data"&gt;


> 

&lt;xsd:element name="channel"&gt;


> > 

&lt;xsd:attribute name=”port” type=”xsd:string” /&gt;


> > 

&lt;xsd:attribute name=”name” type=”xsd:string” /&gt;


> > 

&lt;xsd:attribute name=”noUsers” type=”xsd:string” /&gt;



> 

&lt;/xsd:element&gt;


> 

&lt;/xsd:element&gt;



> Error Response: An error response will include the “Error” status, and a
> comment describing the error.

3.1.2 CreateChan

> Example:  <Request command=”CreateChan” clientname=”foo” data=”channel name”/>

> Success Response: A successful response will include the “Success”
> status in the response message, empty comment, and empty data

> Error Response: An error response will include the “Error” status, and a
> comment describing the error.

3.1.3 DestChan

> Example: <Request command=” DestChan” data=”channel name”/>

> Success Response: A successful response will include the “Success”
> status in the response message, empty comment, and empty data

> Error Response: An error response will include the “Error” status, and a
> comment describing the error.

3.1.5 KillClient

> Example: 

&lt;Request command=”KillClient” data=”foo”/&gt;



> Success Response: A successful response will include the “Success”
> status in the response message, empty comment, and empty data

> Error Response: An error response will include the “Error” status, and a
> comment describing the error.

3.1.6 SetMaxChan

> Example: 

&lt;Request command=”SetMaxChan” data=”40”/&gt;



> Success Response: A successful response will include the “Success”
> status in the response message, empty comment, and empty data

> Error Response: An error response will include the “Error” status, and a
> comment describing the error.

3.1.7 SetMaxClients

> Example: 

&lt;Request command=”SetMaxClients” data=”40”/&gt;



> Success Response: A successful response will include the “Success”
> status in the response message, empty comment, and empty data

> Error Response: An error response will include the “Error” status, and a
> comment describing the error.

3.1.8 SysMessage

> Example: <Request command=”SysMessage” data=”words of message here”/>

> Success Response: A successful response will include the “Success”
> status in the response message, empty comment, and empty data

> Error Response: An error response will include the “Error” status, and a
> comment describing the error.

3.1.9 Version

> Example: 

&lt;Request command=”Version” /&gt;



> Success Response: A successful response will include the “Success”
> status in the response message, the version identifier in the comment,
> and empty data

> Error Response: An error response will include the “Error” status, and a
> comment describing the error.

3.2 Channel

3.2.1 AcceptClient

> Example: 

&lt;Request command=”AcceptClient” clientname=”foo” /&gt;



> Success Response: A successful response will include the “Success”
> status in the response message, the version identifier in the comment,
> and empty data

> Error Response: An error response will include the “Error” status, and a
> comment describing the error.


3.2.2 CloseConn

> Example: 

&lt;Request command=”CloseConn” /&gt;



> Success Response: A successful response will include the “Success”
> status in the response message, description comment, and empty data

> Error Response: An error response will include the “Error” status, and a
> comment describing the error.


3.2.3 EnumClients

> Example: 

&lt;Request command=”EnumClients” /&gt;



> Success Response: A successful response will include the “Success”
> status in the response message, the version identifier in the comment,
> and a nested list of the clients as shown in the data element

> 

&lt;xsd:element name="data"&gt;


> > 

&lt;xsd:element name="clients"&gt;


> > > 

&lt;xsd:attribute name=”name” type=”xsd:string” /&gt;


> > > 

&lt;xsd:attribute name=”id” type=”xsd:string” /&gt;


> > > 

&lt;xsd:attribute name=”channel” type=”xsd:string” /&gt;



> > 

&lt;/xsd:element&gt;



> 

&lt;/xsd:element&gt;



> Error Response: An error response will include the “Error” status, and a
> comment describing the error.

3.2.4 PostMessage

> Example: <Request command=”PostMessage” clientname=”foo” data=”message
> here” />

> Success Response: A successful response will include the “Success”
> status in the response message, empty comment, and empty data

> Error Response: An error response will include the “Error” status, and a
> comment describing the error.

3.2.5 killClient

> Example: <Request command=”killClient” data=”client name” />

> Success Response: A successful response will include the “Success”
> status in the response message, empty comment, and empty data

> Error Response: An error response will include the “Error” status, and a
> comment describing the error.

3.2.6 SetMaxClients

> Example: 

&lt;Request command=”SetMaxClients” data=”40” /&gt;



> Success Response: A successful response will include the “Success”
> status in the response message, empty comment, and empty data

> Error Response: An error response will include the “Error” status, and a
> comment describing the error.


3.3 Client

3.3.1 AcceptMessage

> Example: <Request command=”AcceptMessage” data=”message text here” />

> Success Response: A successful response will include the “Success”
> status in the response message, empty comment, and empty data

> Error Response: An error response will include the “Error” status, and a
> comment describing the error.



4. Message Schema

> <xsd:schema xmlns:xsd="http://www.w3.org/2001/XMLSchema">
> 

&lt;xsd:element name="Message"&gt;


> > 

&lt;xsd:element name="Response"&gt;


> > > 

&lt;xsd:attribute name=”status” type=”xsd:string” /&gt;


> > > 

&lt;xsd:attribute name=”comment” type=”xsd:string” /&gt;


> > > 

&lt;xsd:element name="data"  type=”xsd:string”&gt;


> > > > 

&lt;xsd:element name="channel"  type=”xsd:string”&gt;


> > > > > 

&lt;xsd:attribute name=”port” type=”xsd:string” /&gt;


> > > > > 

&lt;xsd:attribute name=”name” type=”xsd:string” /&gt;


> > > > > 

&lt;xsd:attribute name=”noUsers” type=”xsd:string” /&gt;



> > > > 

&lt;/xsd:element&gt;



> > > 

&lt;/xsd:element&gt;



> > 

&lt;/xsd:element&gt;


> > 

&lt;xsd:element name="Response"&gt;


> > > 

&lt;xsd:attribute name=”status” type=”xsd:string” /&gt;


> > > 

&lt;xsd:attribute name=”comment” type=”xsd:string” /&gt;


> > > > 

&lt;xsd:element name="data"  type=”xsd:string”&gt;


> > > > > 

&lt;xsd:element name="clients"  type=”xsd:string”&gt;


> > > > > > 

&lt;xsd:attribute name=”name” type=”xsd:string” /&gt;


> > > > > > 

&lt;xsd:attribute name=”channel” type=”xsd:string” /&gt;


> > > > > > 

&lt;xsd:attribute name=”id” type=”xsd:string” /&gt;



> > > > > 

&lt;/xsd:element&gt;



> > > > 

&lt;/xsd:element&gt;



> > 

&lt;/xsd:element&gt;



> 

&lt;/xsd:element&gt;


> 

Unknown end tag for &lt;/schema&gt;




5. Replies

> Text based replies from the server, will be non-cryptic full text messages.

6. Authors’ Address

> Jared Sund
> 820 SW Overlook Ct.
> Gresham Or, 97080
> USA

> jsund@pdx.edu
