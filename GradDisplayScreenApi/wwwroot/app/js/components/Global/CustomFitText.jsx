import React, { Component } from 'react';
import { Textfit } from 'react-textfit';

export const CustomFitText = React.createClass({
    getInitialState: function () {
        return { fullname: '' };
    },
    componentDidMount: function () {

        this.state.fullname = this.props.value;

    },

    render: function () {
        
        return <div>
            <Textfit min={30} max={60} mode="single" autoResize={true}>
                {this.props.value}
            </Textfit>
            
        </div>;
    }
});