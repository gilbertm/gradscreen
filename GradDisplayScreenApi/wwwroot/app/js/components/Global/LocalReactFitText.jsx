import React, { Component } from 'react';
import ReactDOM from 'react-dom';
import ReactFitText from 'react-fittext';

export const LocalReactFitText = React.createClass({
  render: function() {
    return <div id="react-root">
      <ReactFitText compressor={0.5}>
        <h1>React FitText</h1>
      </ReactFitText>
      <ReactFitText compressor={3.5}>
        <p className="download">
          A React component for inflating web type
          <a className="download__link" href="http://github.com/gianu/react-fittext">
            Fork it on Github
          </a>
        </p>
      </ReactFitText>
    </div>;
  }
});