import React, { Component } from 'react';
import { Textfit } from 'react-textfit';

import { CustomFitText } from './CustomFitText';


export const Teleprompt = React.createClass({


    // initialize the teleprompt object
    getInitialState: function () {
        return { beingDisplayedGraduate: [], beingDisplayedGraduateId: 0, firstRun: 1 };
    },

    componentDidMount: function () {

        var _this = this;

        setInterval(this.loadFromServer, 500);

        /* document.addEventListener('ended', function (e) {
            e.stopPropagation();

            _this.audioEnded();

        }, true); */

        console.log('loaded');

    },

    // currently displayed graduate object
    getBeingDisplayedGraduate: function () {
        return this.state.beingDisplayedGraduate;
    },

    // firt run will display the initial screen
    // TODO:// automatic display of init page after last graduate
    getFirstRun: function () {
        return this.state.firstRun;
    },

    // mark the teleprompt status to 1
    // for cleanup and auto-reloading of 
    // graduates from the queue (if not empty)
    /* audioEnded: function () {

        $.ajax({
            type: 'POST',
            url: '/api/teleprompt/set/status',
            timeout: 500,
            data: { "status": 0 }
        })
            .done(function (responseText) {
                // console.log("graduate status updated: " + responseText);
            })
            .fail(function (error) {
                // console.log(error);
            });

        // console.log('here' + Math.random());
    }, */

    // check in an interval 
    // of graduate object changes
    // as per dynamically created and/or forced events
    loadFromServer: function () {

        var _this = this;

        fetch('/api/graduate').then(response => response.json()).then(json => {

            // trigger state change
            // empty graduate teleprompt (screen data)
            if (json == null) {
                this.setState({ beingDisplayedGraduate: null });
            }

            if (json != null) {

                $.ajax({
                    type: 'POST',
                    url: '/api/config/set/initialscreen',
                    timeout: 500,
                    data: { "initialscreen": 0 }
                })
                    .done(function (response) {
                        _this.setState({ firstRun: 0 });
                    })
                    .fail(function (error) {
                        // console.log(error);
                    });

                // trigger state change
                // update the currently displayed graduate with the json result
                if (_this.getBeingDisplayedGraduate() != null) {
                    if (json.graduateId != _this.getBeingDisplayedGraduate().graduateId) {

                        _this.setState({ beingDisplayedGraduate: json });

                        // console.log("changed graduate object");
                    }

                } else {

                    _this.setState({ beingDisplayedGraduate: json });

                    // console.log("empty screen");
                }
            }

        });
    },

    render: function () {
        
        var css,
            media,
            fullname,
            credential,
            academic,
            schoolFlag,
            fallingDate,
            fallingMaple;

        if (this.getBeingDisplayedGraduate()) {
            media = (<div>
                {this.getBeingDisplayedGraduate().graduatePicture ?
                    <img src={this.getBeingDisplayedGraduate().graduatePicture} />
                    : ''}

                {this.getBeingDisplayedGraduate().graduateSound ?
                    <audio id="graduateAudio" preload="auto" src={this.getBeingDisplayedGraduate().graduateSound} autoPlay controls></audio>
                    : ''}

                {this.getBeingDisplayedGraduate().merit ?
                    <span className="merit">{this.getBeingDisplayedGraduate().merit}</span>
                    : <span></span>}
            </div>);

            credential = (<div>
                {this.getBeingDisplayedGraduate().graduateId ?
                    <span className="graduateid">{this.getBeingDisplayedGraduate().graduateId}</span>
                    : ''}
            </div>);

            fullname = (<div>
                {this.getBeingDisplayedGraduate().isFullname == 'YES' ?
                    (<div>
                        <div className="arabic-fullname"><CustomFitText value={this.getBeingDisplayedGraduate().arabicFullname} /></div>
                        <div className="fullname"><CustomFitText value={this.getBeingDisplayedGraduate().fullname} /></div>
                    </div>)
                    :
                    (<div>
                        <span className="first">{this.getBeingDisplayedGraduate().firstName}</span>
                        <span className="middle">{this.getBeingDisplayedGraduate().middleName}</span>
                        <span className="last">{this.getBeingDisplayedGraduate().lastName}</span>
                    </div>)
                }
            </div>);

            academic = (
                <div>
                    {this.getBeingDisplayedGraduate().program ?
                        <span className="program">{this.getBeingDisplayedGraduate().program}</span>
                        : ''
                    }

                    {this.getBeingDisplayedGraduate().major ?
                        <span className="major">{this.getBeingDisplayedGraduate().major}</span>
                        : ''
                    }
                </div>);

            schoolFlag = (
                <div>
                    {this.getBeingDisplayedGraduate().graduateImageSystemTemplateFlag ?
                        <img id="flag" className="flag" src={this.getBeingDisplayedGraduate().graduateImageSystemTemplateFlag} />
                        : ''
                    }

                </div>);

            fallingDate = (
                <div>
                    {this.getBeingDisplayedGraduate().graduateImageSystemTemplateDate ?
                        <span id="date" className="date">{this.getBeingDisplayedGraduate().graduateImageSystemTemplateDate}</span>
                        : ''
                    }

                </div>);


            fallingMaple = (
                <div>
                    {this.getBeingDisplayedGraduate().graduateImageSystemTemplateMaple ?
                        <span id="maple" className="maple">{this.getBeingDisplayedGraduate().graduateImageSystemTemplateMaple}</span>
                        : ''
                    }
                </div>);

        } else {

            media = (<div></div>);

            credential = (<div></div>);

            fullname = (<div></div>);

            academic = (<div></div>);

            fallingDate = (
                <div>
                    <span id="date" className="date">/images/system/1920x800/date-default.png</span>
                </div>);


            fallingMaple = (
                <div>
                    <span id="maple" className="maple">/images/system/1920x800/maple-default.png</span>
                </div>);
        }

        return <div>
            <div className={"main-wrapper " + (this.getFirstRun() ? 'firstRun' : 'running')}>
                <div className="container">
                    <div className="media">
                        {media}
                    </div>
                    <div className="profile">
                        
                        <div id="fullname">
                            {fullname}
                        </div>

                        <div className="academic">
                            {academic}
                        </div>
                    </div>
                </div>

                <div className="school-flag">
                    {schoolFlag}
                    {fallingDate}
                    {fallingMaple}
                </div>

            </div>
        </div>;
    }
});

